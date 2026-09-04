using System.Buffers.Binary;
using System.Text.Json;

namespace ArenaSystemsLab.Server;

public enum RequestKind
{
    Health,
    SubmitScore,
    GetLeaderboard
}

public sealed record ArenaRequest(RequestKind Kind, string? PlayerId = null, int Score = 0, int Limit = 0);

public sealed class ProtocolException(string code) : Exception(code)
{
    public string Code { get; } = code;
}

public static class WireProtocol
{
    public const int Version = 1;
    public const int MaxPayloadBytes = 16 * 1024;
    public const int MaxPlayerIdLength = 32;
    public const int MaxScore = 1_000_000;
    public const int MaxLeaderboardEntries = 100;

    private const int PrefixBytes = 4;
    private static readonly JsonDocumentOptions DocumentOptions = new()
    {
        AllowTrailingCommas = false,
        CommentHandling = JsonCommentHandling.Disallow,
        MaxDepth = 8
    };
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async ValueTask<byte[]> ReadFrameAsync(Stream stream, CancellationToken cancellationToken)
    {
        byte[] prefix = new byte[PrefixBytes];
        try
        {
            await stream.ReadExactlyAsync(prefix, cancellationToken);
        }
        catch (EndOfStreamException)
        {
            throw new ProtocolException("incomplete_frame");
        }

        int payloadLength = BinaryPrimitives.ReadInt32BigEndian(prefix);
        if (payloadLength is <= 0 or > MaxPayloadBytes)
        {
            throw new ProtocolException("invalid_frame_length");
        }

        byte[] payload = new byte[payloadLength];
        try
        {
            await stream.ReadExactlyAsync(payload, cancellationToken);
        }
        catch (EndOfStreamException)
        {
            throw new ProtocolException("incomplete_frame");
        }

        return payload;
    }

    public static async ValueTask WriteFrameAsync(
        Stream stream,
        ReadOnlyMemory<byte> payload,
        CancellationToken cancellationToken)
    {
        if (payload.Length is <= 0 or > MaxPayloadBytes)
        {
            throw new ProtocolException("invalid_frame_length");
        }

        byte[] prefix = new byte[PrefixBytes];
        BinaryPrimitives.WriteInt32BigEndian(prefix, payload.Length);
        await stream.WriteAsync(prefix, cancellationToken);
        await stream.WriteAsync(payload, cancellationToken);
    }

    public static ArenaRequest ParseRequest(ReadOnlyMemory<byte> payload)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(payload, DocumentOptions);
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                throw new ProtocolException("invalid_request");
            }

            var properties = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
            foreach (JsonProperty property in root.EnumerateObject())
            {
                if (!properties.TryAdd(property.Name, property.Value))
                {
                    throw new ProtocolException("duplicate_property");
                }
            }

            int version = ReadRequiredInt(properties, "version");
            if (version != Version)
            {
                throw new ProtocolException("unsupported_version");
            }

            string type = ReadRequiredString(properties, "type");
            return type switch
            {
                "health" => ParseHealth(properties),
                "submit_score" => ParseSubmitScore(properties),
                "get_leaderboard" => ParseGetLeaderboard(properties),
                _ => throw new ProtocolException("unsupported_type")
            };
        }
        catch (JsonException)
        {
            throw new ProtocolException("invalid_json");
        }
    }

    public static byte[] Serialize(object value)
    {
        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(value, SerializerOptions);
        if (payload.Length > MaxPayloadBytes)
        {
            throw new ProtocolException("response_too_large");
        }

        return payload;
    }

    private static ArenaRequest ParseHealth(Dictionary<string, JsonElement> properties)
    {
        RequireExactPropertyCount(properties, 2);
        return new ArenaRequest(RequestKind.Health);
    }

    private static ArenaRequest ParseSubmitScore(Dictionary<string, JsonElement> properties)
    {
        RequireExactPropertyCount(properties, 4);
        string playerId = ReadRequiredString(properties, "playerId");
        if (!IsValidPlayerId(playerId))
        {
            throw new ProtocolException("invalid_player_id");
        }

        int score = ReadRequiredInt(properties, "score");
        if (score is < 0 or > MaxScore)
        {
            throw new ProtocolException("invalid_score");
        }

        return new ArenaRequest(RequestKind.SubmitScore, playerId, score);
    }

    private static ArenaRequest ParseGetLeaderboard(Dictionary<string, JsonElement> properties)
    {
        RequireExactPropertyCount(properties, 3);
        int limit = ReadRequiredInt(properties, "limit");
        if (limit is < 1 or > MaxLeaderboardEntries)
        {
            throw new ProtocolException("invalid_limit");
        }

        return new ArenaRequest(RequestKind.GetLeaderboard, Limit: limit);
    }

    private static int ReadRequiredInt(Dictionary<string, JsonElement> properties, string name)
    {
        if (!properties.TryGetValue(name, out JsonElement value) || !value.TryGetInt32(out int result))
        {
            throw new ProtocolException("invalid_request");
        }

        return result;
    }

    private static string ReadRequiredString(Dictionary<string, JsonElement> properties, string name)
    {
        if (!properties.TryGetValue(name, out JsonElement value) || value.ValueKind != JsonValueKind.String)
        {
            throw new ProtocolException("invalid_request");
        }

        return value.GetString() ?? throw new ProtocolException("invalid_request");
    }

    private static void RequireExactPropertyCount(Dictionary<string, JsonElement> properties, int expected)
    {
        if (properties.Count != expected)
        {
            throw new ProtocolException("invalid_request");
        }
    }

    private static bool IsValidPlayerId(string playerId)
    {
        if (playerId.Length is < 1 or > MaxPlayerIdLength)
        {
            return false;
        }

        foreach (char character in playerId)
        {
            bool allowed = character is >= 'a' and <= 'z'
                or >= 'A' and <= 'Z'
                or >= '0' and <= '9'
                or '_'
                or '-';
            if (!allowed)
            {
                return false;
            }
        }

        return true;
    }
}
