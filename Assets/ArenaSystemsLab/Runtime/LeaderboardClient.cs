using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ArenaSystemsLab
{
    public sealed class LeaderboardClient
    {
        public const int DefaultPort = 7777;
        public const int MaxPayloadBytes = 16 * 1024;
        public const int MaxPlayerIdLength = 32;
        public const int MaxScore = 1_000_000;
        public const int MaxLeaderboardEntries = 100;

        private const int ProtocolVersion = 1;
        private const int PrefixBytes = 4;
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(3);
        private static readonly TimeSpan RetryDelay = TimeSpan.FromMilliseconds(250);
        private static readonly UTF8Encoding StrictUtf8 = new UTF8Encoding(false, true);

        private readonly int port;
        private readonly TimeSpan requestTimeout;

        public LeaderboardClient(int port = DefaultPort, TimeSpan? requestTimeout = null)
        {
            if (port is < 1 or > 65535)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            this.requestTimeout = requestTimeout ?? DefaultTimeout;
            if (this.requestTimeout <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(requestTimeout));
            }

            this.port = port;
        }

        public async Task<LeaderboardEntry[]> SubmitAndGetLeaderboardAsync(
            string playerId,
            int score,
            int limit,
            CancellationToken cancellationToken)
        {
            ValidatePlayerId(playerId);
            if (score is < 0 or > MaxScore)
            {
                throw new ArgumentOutOfRangeException(nameof(score));
            }

            if (limit is < 1 or > MaxLeaderboardEntries)
            {
                throw new ArgumentOutOfRangeException(nameof(limit));
            }

            for (int attempt = 0; ; attempt++)
            {
                try
                {
                    await SubmitScoreAsync(playerId, score, cancellationToken).ConfigureAwait(false);
                    return await GetLeaderboardAsync(limit, cancellationToken).ConfigureAwait(false);
                }
                catch (LeaderboardClientException exception) when (attempt == 0 && IsRetryable(exception.Code))
                {
                    await Task.Delay(RetryDelay, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task SubmitScoreAsync(string playerId, int score, CancellationToken cancellationToken)
        {
            var request = new SubmitScoreRequest(ProtocolVersion, "submit_score", playerId, score);
            SubmitScoreResponse response = await SendAsync<SubmitScoreResponse>(request, cancellationToken)
                .ConfigureAwait(false);
            RequireSuccess(response.version, response.ok);
            if (response.bestScore < score || response.bestScore > MaxScore)
            {
                throw new LeaderboardClientException("invalid_response");
            }
        }

        private async Task<LeaderboardEntry[]> GetLeaderboardAsync(int limit, CancellationToken cancellationToken)
        {
            var request = new GetLeaderboardRequest(ProtocolVersion, "get_leaderboard", limit);
            LeaderboardResponse response = await SendAsync<LeaderboardResponse>(request, cancellationToken)
                .ConfigureAwait(false);
            RequireSuccess(response.version, response.ok);

            LeaderboardEntry[] entries = response.entries;
            if (entries == null || entries.Length > limit)
            {
                throw new LeaderboardClientException("invalid_response");
            }

            for (int index = 0; index < entries.Length; index++)
            {
                LeaderboardEntry entry = entries[index];
                if (entry == null || !IsValidPlayerId(entry.PlayerId) || entry.Score is < 0 or > MaxScore)
                {
                    throw new LeaderboardClientException("invalid_response");
                }

                if (index == 0)
                {
                    continue;
                }

                LeaderboardEntry previous = entries[index - 1];
                bool wrongScoreOrder = previous.Score < entry.Score;
                bool wrongTieOrder = previous.Score == entry.Score
                    && string.CompareOrdinal(previous.PlayerId, entry.PlayerId) > 0;
                if (wrongScoreOrder || wrongTieOrder)
                {
                    throw new LeaderboardClientException("invalid_response");
                }
            }

            return entries;
        }

        private async Task<TResponse> SendAsync<TResponse>(object request, CancellationToken cancellationToken)
        {
            byte[] requestPayload = StrictUtf8.GetBytes(JsonUtility.ToJson(request));
            if (requestPayload.Length is <= 0 or > MaxPayloadBytes)
            {
                throw new LeaderboardClientException("invalid_request");
            }

            using (var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            using (var client = new TcpClient(AddressFamily.InterNetwork))
            {
                timeout.CancelAfter(requestTimeout);
                try
                {
                    await ConnectAsync(client, timeout.Token).ConfigureAwait(false);
                    NetworkStream stream = client.GetStream();
                    await WriteFrameAsync(stream, requestPayload, timeout.Token).ConfigureAwait(false);
                    byte[] responsePayload = await ReadFrameAsync(stream, timeout.Token).ConfigureAwait(false);
                    string responseJson = StrictUtf8.GetString(responsePayload);
                    TResponse response = JsonUtility.FromJson<TResponse>(responseJson);
                    if (response == null)
                    {
                        throw new LeaderboardClientException("invalid_response");
                    }

                    return response;
                }
                catch (LeaderboardClientException)
                {
                    throw;
                }
                catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
                {
                    throw new LeaderboardClientException("request_timeout");
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (SocketException exception)
                {
                    throw new LeaderboardClientException("connection_failed", exception);
                }
                catch (IOException exception)
                {
                    throw new LeaderboardClientException("connection_io_error", exception);
                }
                catch (DecoderFallbackException exception)
                {
                    throw new LeaderboardClientException("invalid_response", exception);
                }
                catch (ArgumentException exception)
                {
                    throw new LeaderboardClientException("invalid_response", exception);
                }
            }
        }

        private async Task ConnectAsync(TcpClient client, CancellationToken cancellationToken)
        {
            Task connect = client.ConnectAsync(IPAddress.Loopback, port);
            Task cancelled = Task.Delay(Timeout.Infinite, cancellationToken);
            if (await Task.WhenAny(connect, cancelled).ConfigureAwait(false) != connect)
            {
                client.Close();
                try
                {
                    await connect.ConfigureAwait(false);
                }
                catch (Exception)
                {
                }

                cancellationToken.ThrowIfCancellationRequested();
            }

            await connect.ConfigureAwait(false);
        }

        private static async Task WriteFrameAsync(Stream stream, byte[] payload, CancellationToken cancellationToken)
        {
            byte[] prefix =
            {
                (byte)(payload.Length >> 24),
                (byte)(payload.Length >> 16),
                (byte)(payload.Length >> 8),
                (byte)payload.Length
            };
            await stream.WriteAsync(prefix, 0, prefix.Length, cancellationToken).ConfigureAwait(false);
            await stream.WriteAsync(payload, 0, payload.Length, cancellationToken).ConfigureAwait(false);
        }

        private static async Task<byte[]> ReadFrameAsync(Stream stream, CancellationToken cancellationToken)
        {
            byte[] prefix = new byte[PrefixBytes];
            await ReadExactlyAsync(stream, prefix, cancellationToken).ConfigureAwait(false);
            int payloadLength = prefix[0] << 24
                | prefix[1] << 16
                | prefix[2] << 8
                | prefix[3];
            if (payloadLength is <= 0 or > MaxPayloadBytes)
            {
                throw new LeaderboardClientException("invalid_frame_length");
            }

            byte[] payload = new byte[payloadLength];
            await ReadExactlyAsync(stream, payload, cancellationToken).ConfigureAwait(false);
            return payload;
        }

        private static async Task ReadExactlyAsync(Stream stream, byte[] buffer, CancellationToken cancellationToken)
        {
            int offset = 0;
            while (offset < buffer.Length)
            {
                int read = await stream.ReadAsync(buffer, offset, buffer.Length - offset, cancellationToken)
                    .ConfigureAwait(false);
                if (read == 0)
                {
                    throw new LeaderboardClientException("incomplete_frame");
                }

                offset += read;
            }
        }

        private static void RequireSuccess(int version, bool ok)
        {
            if (version != ProtocolVersion || !ok)
            {
                throw new LeaderboardClientException("server_rejected_request");
            }
        }

        private static void ValidatePlayerId(string playerId)
        {
            if (!IsValidPlayerId(playerId))
            {
                throw new ArgumentException("Player ID must contain 1-32 ASCII letters, digits, '_' or '-'.", nameof(playerId));
            }
        }

        private static bool IsValidPlayerId(string playerId)
        {
            if (string.IsNullOrEmpty(playerId) || playerId.Length > MaxPlayerIdLength)
            {
                return false;
            }

            for (int index = 0; index < playerId.Length; index++)
            {
                char character = playerId[index];
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

        private static bool IsRetryable(string code)
        {
            return code is "connection_failed" or "connection_io_error" or "request_timeout" or "incomplete_frame";
        }

        [Serializable]
        private sealed class SubmitScoreRequest
        {
            public int version;
            public string type;
            public string playerId;
            public int score;

            public SubmitScoreRequest(int version, string type, string playerId, int score)
            {
                this.version = version;
                this.type = type;
                this.playerId = playerId;
                this.score = score;
            }
        }

        [Serializable]
        private sealed class GetLeaderboardRequest
        {
            public int version;
            public string type;
            public int limit;

            public GetLeaderboardRequest(int version, string type, int limit)
            {
                this.version = version;
                this.type = type;
                this.limit = limit;
            }
        }

        [Serializable]
        private sealed class SubmitScoreResponse
        {
            public int version;
            public bool ok;
            public int bestScore;
        }

        [Serializable]
        private sealed class LeaderboardResponse
        {
            public int version;
            public bool ok;
            public LeaderboardEntry[] entries;
        }
    }

    [Serializable]
    public sealed class LeaderboardEntry
    {
        [SerializeField] private string playerId;
        [SerializeField] private int score;

        public string PlayerId => playerId;
        public int Score => score;
    }

    public sealed class LeaderboardClientException : Exception
    {
        public LeaderboardClientException(string code, Exception innerException = null)
            : base($"Leaderboard request failed: {code}", innerException)
        {
            Code = code;
        }

        public string Code { get; }
    }
}
