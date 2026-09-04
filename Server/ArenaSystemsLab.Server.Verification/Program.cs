using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using ArenaSystemsLab.Server;

return await ServerVerification.RunAsync();

internal static class ServerVerification
{
    public static async Task<int> RunAsync()
    {
        (string Name, Func<Task> Run)[] checks =
        [
            ("fragmented frame", FrameCodecReadsFragmentedStreamAsync),
            ("invalid frame length", FrameCodecRejectsInvalidLengthAsync),
            ("strict request validation", ParserRejectsUnexpectedInputAsync),
            ("bounded store", StoreRejectsUnboundedPlayersAsync),
            ("multithreaded store", StoreRemainsConsistentAcrossThreadsAsync),
            ("loopback health", ServerAnswersHealthAsync),
            ("slow client timeout", ServerDisconnectsSlowClientAsync),
            ("concurrent clients", ServerHandlesConcurrentScoresAsync)
        ];

        int failures = 0;
        foreach ((string name, Func<Task> run) in checks)
        {
            try
            {
                await run();
                Console.WriteLine($"PASS {name}");
            }
            catch (Exception exception)
            {
                failures++;
                Console.Error.WriteLine($"FAIL {name}: {exception.Message}");
            }
        }

        Console.WriteLine($"RESULT passed={checks.Length - failures} failed={failures}");
        return failures == 0 ? 0 : 1;
    }

    private static async Task FrameCodecReadsFragmentedStreamAsync()
    {
        byte[] payload = WireProtocol.Serialize(new { version = 1, type = "health" });
        byte[] frame = CreateFrame(payload);
        await using var stream = new OneByteReadStream(frame);
        byte[] actual = await WireProtocol.ReadFrameAsync(stream, CancellationToken.None);
        Assert(payload.SequenceEqual(actual), "Fragmented frame payload changed.");
    }

    private static async Task FrameCodecRejectsInvalidLengthAsync()
    {
        await using var emptyStream = new MemoryStream(new byte[4]);
        await AssertProtocolErrorAsync(
            "invalid_frame_length",
            () => WireProtocol.ReadFrameAsync(emptyStream, CancellationToken.None).AsTask());

        byte[] oversizedPrefix = new byte[4];
        BinaryPrimitives.WriteInt32BigEndian(oversizedPrefix, WireProtocol.MaxPayloadBytes + 1);
        await using var oversizedStream = new MemoryStream(oversizedPrefix);
        await AssertProtocolErrorAsync(
            "invalid_frame_length",
            () => WireProtocol.ReadFrameAsync(oversizedStream, CancellationToken.None).AsTask());
    }

    private static Task ParserRejectsUnexpectedInputAsync()
    {
        AssertProtocolError(
            "invalid_request",
            () => WireProtocol.ParseRequest(WireProtocol.Serialize(new
            {
                version = 1,
                type = "health",
                unexpected = true
            })));
        AssertProtocolError(
            "invalid_player_id",
            () => WireProtocol.ParseRequest(WireProtocol.Serialize(new
            {
                version = 1,
                type = "submit_score",
                playerId = "bad player",
                score = 10
            })));
        AssertProtocolError(
            "invalid_score",
            () => WireProtocol.ParseRequest(WireProtocol.Serialize(new
            {
                version = 1,
                type = "submit_score",
                playerId = "valid-player",
                score = WireProtocol.MaxScore + 1
            })));
        AssertProtocolError(
            "duplicate_property",
            () => WireProtocol.ParseRequest(System.Text.Encoding.UTF8.GetBytes(
                "{\"version\":1,\"type\":\"health\",\"type\":\"health\"}")));
        return Task.CompletedTask;
    }

    private static Task StoreRemainsConsistentAcrossThreadsAsync()
    {
        const int threadCount = 8;
        const int submissionsPerThread = 500;
        var store = new LeaderboardStore();
        using var start = new ManualResetEventSlim(false);
        var threads = new Thread[threadCount];

        for (int index = 0; index < threads.Length; index++)
        {
            int worker = index;
            threads[index] = new Thread(() =>
            {
                start.Wait();
                for (int submission = 0; submission < submissionsPerThread; submission++)
                {
                    store.SubmitScore("shared-player", worker * submissionsPerThread + submission);
                }
            })
            {
                IsBackground = true
            };
            threads[index].Start();
        }

        start.Set();
        foreach (Thread thread in threads)
        {
            Assert(thread.Join(TimeSpan.FromSeconds(5)), "Worker thread did not finish.");
        }

        ScoreEntry[] top = store.GetTop(1);
        Assert(top.Length == 1, "Expected one leaderboard entry.");
        Assert(top[0].Score == threadCount * submissionsPerThread - 1, "Highest concurrent score was lost.");
        return Task.CompletedTask;
    }

    private static Task StoreRejectsUnboundedPlayersAsync()
    {
        var store = new LeaderboardStore(2);
        store.SubmitScore("player-1", 1);
        store.SubmitScore("player-2", 2);
        AssertProtocolError(
            "leaderboard_capacity_reached",
            () => store.SubmitScore("player-3", 3));
        Assert(store.SubmitScore("player-1", 4) == 4, "Existing player could not update at capacity.");
        return Task.CompletedTask;
    }

    private static Task ServerAnswersHealthAsync()
    {
        return WithServerAsync(async port =>
        {
            JsonElement response = await SendAsync(port, new { version = 1, type = "health" });
            Assert(response.GetProperty("ok").GetBoolean(), "Health response was not successful.");
            Assert(response.GetProperty("type").GetString() == "health", "Health response type changed.");
        });
    }

    private static Task ServerDisconnectsSlowClientAsync()
    {
        return WithServerAsync(async port =>
        {
            using var client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, port);
            NetworkStream stream = client.GetStream();
            await stream.WriteAsync(new byte[] { 0, 0 });

            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(2));
            byte[] buffer = new byte[1];
            int read = await stream.ReadAsync(buffer, timeout.Token);
            Assert(read == 0, "Slow client connection stayed open after the request timeout.");
        }, TimeSpan.FromMilliseconds(200));
    }

    private static Task ServerHandlesConcurrentScoresAsync()
    {
        return WithServerAsync(async port =>
        {
            Task<JsonElement>[] submissions = Enumerable.Range(0, 24)
                .Select(index => SendAsync(port, new
                {
                    version = 1,
                    type = "submit_score",
                    playerId = $"player-{index % 4}",
                    score = index
                }))
                .ToArray();
            JsonElement[] responses = await Task.WhenAll(submissions);
            Assert(responses.All(response => response.GetProperty("ok").GetBoolean()), "A score submission failed.");

            JsonElement leaderboard = await SendAsync(port, new
            {
                version = 1,
                type = "get_leaderboard",
                limit = 10
            });
            JsonElement entries = leaderboard.GetProperty("entries");
            Assert(entries.GetArrayLength() == 4, "Expected four players.");
            Assert(entries[0].GetProperty("score").GetInt32() == 23, "Leaderboard ordering or best score is wrong.");
        });
    }

    private static async Task WithServerAsync(Func<int, Task> check, TimeSpan? requestTimeout = null)
    {
        using var shutdown = new CancellationTokenSource();
        var server = new LeaderboardServer(0, 8, requestTimeout ?? TimeSpan.FromSeconds(2));
        Task serverTask = server.RunAsync(shutdown.Token);
        try
        {
            await check(server.Port);
        }
        finally
        {
            shutdown.Cancel();
            await serverTask.WaitAsync(TimeSpan.FromSeconds(5));
        }
    }

    private static async Task<JsonElement> SendAsync(int port, object request)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        using var client = new TcpClient();
        await client.ConnectAsync(IPAddress.Loopback, port, timeout.Token);
        NetworkStream stream = client.GetStream();
        await WireProtocol.WriteFrameAsync(stream, WireProtocol.Serialize(request), timeout.Token);
        byte[] response = await WireProtocol.ReadFrameAsync(stream, timeout.Token);
        using JsonDocument document = JsonDocument.Parse(response);
        return document.RootElement.Clone();
    }

    private static byte[] CreateFrame(byte[] payload)
    {
        byte[] frame = new byte[payload.Length + 4];
        BinaryPrimitives.WriteInt32BigEndian(frame, payload.Length);
        payload.CopyTo(frame, 4);
        return frame;
    }

    private static void AssertProtocolError(string expectedCode, Action action)
    {
        try
        {
            action();
        }
        catch (ProtocolException exception) when (exception.Code == expectedCode)
        {
            return;
        }

        throw new InvalidOperationException($"Expected protocol error {expectedCode}.");
    }

    private static async Task AssertProtocolErrorAsync(string expectedCode, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (ProtocolException exception) when (exception.Code == expectedCode)
        {
            return;
        }

        throw new InvalidOperationException($"Expected protocol error {expectedCode}.");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private sealed class OneByteReadStream(byte[] buffer) : MemoryStream(buffer)
    {
        public override ValueTask<int> ReadAsync(
            Memory<byte> destination,
            CancellationToken cancellationToken = default)
        {
            return base.ReadAsync(destination[..Math.Min(1, destination.Length)], cancellationToken);
        }
    }
}
