using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ArenaSystemsLab.Tests.EditMode
{
    public sealed class LeaderboardClientTests
    {
        private const string ZeroScoreResponse = "{\"version\":1,\"ok\":true,\"bestScore\":0}";

        [TestCase("{\"version\":1,\"ok\":true}", null)]
        [TestCase("{\"version\":1,\"ok\":true,\"bestScore\":-1}", null)]
        [TestCase("{\"version\":1,\"ok\":true,\"bestScore\":1000001}", null)]
        [TestCase(ZeroScoreResponse, "{\"version\":1,\"ok\":true,\"entries\":[{\"playerId\":\"UnityPlayer\"}]}")]
        [TestCase(ZeroScoreResponse, "{\"version\":1,\"ok\":true,\"entries\":[{\"playerId\":\"UnityPlayer\",\"score\":0},{\"playerId\":\"UnityPlayer\",\"score\":0}]}")]
        [TestCase(ZeroScoreResponse, "{\"version\":1,\"ok\":true,\"entries\":[{\"playerId\":\"UnityPlayer\",\"score\":3},{\"playerId\":\"OtherPlayer\",\"score\":2},{\"playerId\":\"UnityPlayer\",\"score\":1}]}")]
        public void SubmitAndGetLeaderboard_WithInvalidSuccessResponse_RejectsResponse(string submitResponse, string queryResponse)
        {
            Task.Run(() => RunResponseCheckAsync(submitResponse, queryResponse, false)).GetAwaiter().GetResult();
        }

        [TestCase("{\"version\":1,\"ok\":true,\"entries\":[{\"playerId\":\"UnityPlayer\",\"score\":0}]}", 1)]
        [TestCase("{\"version\":1,\"ok\":true,\"entries\":[{\"playerId\":\"UnityPlayer\",\"score\":0},{\"playerId\":\"unityPlayer\",\"score\":0}]}", 2)]
        public void SubmitAndGetLeaderboard_WithZeroScoresAndOrdinalIds_AcceptsResponse(string queryResponse, int expectedCount)
        {
            LeaderboardEntry[] entries = Task.Run(() => RunResponseCheckAsync(ZeroScoreResponse, queryResponse, true))
                .GetAwaiter().GetResult();
            Assert.That(entries, Has.Length.EqualTo(expectedCount));
            Assert.That(entries[0].Score, Is.Zero);
        }

        [Test]
        public void SubmitAndGetLeaderboard_UsesBoundedFramedProtocol()
        {
            Task.Run(RunProtocolCheckAsync).GetAwaiter().GetResult();
        }

        [Test]
        public void SubmitAndGetLeaderboard_WhenFirstResponseIsIncomplete_RetriesOnce()
        {
            Task.Run(RunRetryCheckAsync).GetAwaiter().GetResult();
        }

        [Test]
        public void SubmitAndGetLeaderboard_WithOversizedResponse_RejectsFrame()
        {
            Task.Run(RunOversizedFrameCheckAsync).GetAwaiter().GetResult();
        }

        [Test]
        public void SubmitAndGetLeaderboard_WithoutServer_ReportsUnavailable()
        {
            Task.Run(RunMissingServerCheck).GetAwaiter().GetResult();
        }

        private static async Task<LeaderboardEntry[]> RunResponseCheckAsync(
            string submitResponse, string queryResponse, bool accepted)
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            Task<string[]> server = ServeSuccessfulSessionAsync(listener, submitResponse, queryResponse);
            try
            {
                var client = new LeaderboardClient(((IPEndPoint)listener.LocalEndpoint).Port, TimeSpan.FromSeconds(1));
                LeaderboardEntry[] entries = null;
                if (accepted)
                {
                    entries = await client.SubmitAndGetLeaderboardAsync("UnityPlayer", 0, 3, CancellationToken.None);
                }
                else
                {
                    LeaderboardClientException exception = Assert.ThrowsAsync<LeaderboardClientException>(async () =>
                        await client.SubmitAndGetLeaderboardAsync("UnityPlayer", 0, 3, CancellationToken.None));
                    Assert.That(exception.Code, Is.EqualTo("invalid_response"));
                }

                Assert.That(await Task.WhenAny(server, Task.Delay(TimeSpan.FromSeconds(3))), Is.SameAs(server));
                Assert.That(await server, Has.Length.EqualTo(queryResponse == null ? 1 : 2));
                if (queryResponse != null)
                {
                    Assert.That(listener.Pending(), Is.False, "Invalid responses must not trigger a retry.");
                }

                return entries;
            }
            finally
            {
                listener.Stop();
                try
                {
                    await server;
                }
                catch (SocketException)
                {
                    // Observe the pending accept when a failed client assertion closes the test listener.
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        private static async Task RunProtocolCheckAsync()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                Task<string[]> server = ServeSuccessfulSessionAsync(listener);
                var client = new LeaderboardClient(port, TimeSpan.FromSeconds(2));

                LeaderboardEntry[] entries = await client.SubmitAndGetLeaderboardAsync(
                    "UnityPlayer",
                    12,
                    5,
                    CancellationToken.None);
                string[] requests = await server;

                Assert.That(requests[0], Is.EqualTo(
                    "{\"version\":1,\"type\":\"submit_score\",\"playerId\":\"UnityPlayer\",\"score\":12}"));
                Assert.That(requests[1], Is.EqualTo(
                    "{\"version\":1,\"type\":\"get_leaderboard\",\"limit\":5}"));
                Assert.That(entries, Has.Length.EqualTo(2));
                Assert.That(entries[0].PlayerId, Is.EqualTo("UnityPlayer"));
                Assert.That(entries[0].Score, Is.EqualTo(12));
                Assert.That(entries[1].PlayerId, Is.EqualTo("OtherPlayer"));
                Assert.That(entries[1].Score, Is.EqualTo(7));
            }
            finally
            {
                listener.Stop();
            }
        }

        private static async Task RunRetryCheckAsync()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                Task<int> server = ServeRetrySessionAsync(listener);
                var client = new LeaderboardClient(port, TimeSpan.FromSeconds(2));

                LeaderboardEntry[] entries = await client.SubmitAndGetLeaderboardAsync(
                    "UnityPlayer",
                    3,
                    1,
                    CancellationToken.None);

                Assert.That(await server, Is.EqualTo(3), "Expected one failed request and one complete retry.");
                Assert.That(entries, Has.Length.EqualTo(1));
                Assert.That(entries[0].Score, Is.EqualTo(3));
            }
            finally
            {
                listener.Stop();
            }
        }

        private static async Task RunOversizedFrameCheckAsync()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            try
            {
                listener.Start();
                int port = ((IPEndPoint)listener.LocalEndpoint).Port;
                Task server = ServeOversizedFrameAsync(listener);
                var client = new LeaderboardClient(port, TimeSpan.FromSeconds(2));

                LeaderboardClientException exception = Assert.ThrowsAsync<LeaderboardClientException>(async () =>
                    await client.SubmitAndGetLeaderboardAsync(
                        "UnityPlayer",
                        1,
                        1,
                        CancellationToken.None));

                Assert.That(exception.Code, Is.EqualTo("invalid_frame_length"));
                await server;
            }
            finally
            {
                listener.Stop();
            }
        }

        private static void RunMissingServerCheck()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            int unusedPort = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();

            var client = new LeaderboardClient(unusedPort, TimeSpan.FromSeconds(1));
            LeaderboardClientException exception = Assert.ThrowsAsync<LeaderboardClientException>(async () =>
                await client.SubmitAndGetLeaderboardAsync(
                    "UnityPlayer",
                    1,
                    1,
                    CancellationToken.None));

            Assert.That(
                exception.Code == "connection_failed" || exception.Code == "request_timeout",
                Is.True);
        }

        private static async Task<string[]> ServeSuccessfulSessionAsync(
            TcpListener listener,
            string submitResponse = "{\"version\":1,\"ok\":true,\"bestScore\":12}",
            string queryResponse = "{\"version\":1,\"ok\":true,\"entries\":["
                + "{\"playerId\":\"UnityPlayer\",\"score\":12},"
                + "{\"playerId\":\"OtherPlayer\",\"score\":7}]}")
        {
            string submit = await ReceiveAndRespondAsync(listener, submitResponse);
            if (queryResponse == null)
            {
                listener.Stop();
                return new[] { submit };
            }

            string query = await ReceiveAndRespondAsync(listener, queryResponse);
            return new[] { submit, query };
        }

        private static async Task<int> ServeRetrySessionAsync(TcpListener listener)
        {
            int requestCount = 0;
            using (TcpClient first = await listener.AcceptTcpClientAsync())
            {
                await ReadFrameAsync(first.GetStream());
                requestCount++;
            }

            await ReceiveAndRespondAsync(
                listener,
                "{\"version\":1,\"ok\":true,\"bestScore\":3}");
            requestCount++;
            await ReceiveAndRespondAsync(
                listener,
                "{\"version\":1,\"ok\":true,\"entries\":[{\"playerId\":\"UnityPlayer\",\"score\":3}]}");
            requestCount++;
            return requestCount;
        }

        private static async Task ServeOversizedFrameAsync(TcpListener listener)
        {
            using (TcpClient client = await listener.AcceptTcpClientAsync())
            {
                NetworkStream stream = client.GetStream();
                await ReadFrameAsync(stream);
                int length = LeaderboardClient.MaxPayloadBytes + 1;
                byte[] prefix =
                {
                    (byte)(length >> 24),
                    (byte)(length >> 16),
                    (byte)(length >> 8),
                    (byte)length
                };
                await stream.WriteAsync(prefix, 0, prefix.Length);
            }
        }

        private static async Task<string> ReceiveAndRespondAsync(TcpListener listener, string response)
        {
            using (TcpClient client = await listener.AcceptTcpClientAsync())
            {
                NetworkStream stream = client.GetStream();
                string request = Encoding.UTF8.GetString(await ReadFrameAsync(stream));
                await WriteFrameAsync(stream, Encoding.UTF8.GetBytes(response));
                return request;
            }
        }

        private static async Task<byte[]> ReadFrameAsync(Stream stream)
        {
            byte[] prefix = new byte[4];
            await ReadExactlyAsync(stream, prefix);
            int length = prefix[0] << 24 | prefix[1] << 16 | prefix[2] << 8 | prefix[3];
            if (length is <= 0 or > LeaderboardClient.MaxPayloadBytes)
            {
                throw new InvalidDataException("Test server received an invalid frame length.");
            }

            byte[] payload = new byte[length];
            await ReadExactlyAsync(stream, payload);
            return payload;
        }

        private static async Task WriteFrameAsync(Stream stream, byte[] payload)
        {
            byte[] prefix =
            {
                (byte)(payload.Length >> 24),
                (byte)(payload.Length >> 16),
                (byte)(payload.Length >> 8),
                (byte)payload.Length
            };
            await stream.WriteAsync(prefix, 0, prefix.Length);
            await stream.WriteAsync(payload, 0, payload.Length);
        }

        private static async Task ReadExactlyAsync(Stream stream, byte[] buffer)
        {
            int offset = 0;
            while (offset < buffer.Length)
            {
                int read = await stream.ReadAsync(buffer, offset, buffer.Length - offset);
                if (read == 0)
                {
                    throw new EndOfStreamException();
                }

                offset += read;
            }
        }
    }
}
