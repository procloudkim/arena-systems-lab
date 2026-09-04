using System.Net;
using System.Net.Sockets;

namespace ArenaSystemsLab.Server;

public sealed class LeaderboardServer
{
    public const int DefaultMaxConcurrentClients = 16;
    public static readonly TimeSpan DefaultRequestTimeout = TimeSpan.FromSeconds(5);

    private readonly TcpListener listener;
    private readonly SemaphoreSlim clientSlots;
    private readonly LeaderboardStore store = new();
    private readonly int maxConcurrentClients;
    private readonly TimeSpan requestTimeout;
    private bool started;

    public LeaderboardServer(
        int port,
        int maxConcurrentClients = DefaultMaxConcurrentClients,
        TimeSpan? requestTimeout = null)
    {
        if (port is < 0 or > 65535)
        {
            throw new ArgumentOutOfRangeException(nameof(port));
        }

        if (maxConcurrentClients <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxConcurrentClients));
        }

        this.maxConcurrentClients = maxConcurrentClients;
        this.requestTimeout = requestTimeout ?? DefaultRequestTimeout;
        if (this.requestTimeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(requestTimeout));
        }

        listener = new TcpListener(IPAddress.Loopback, port);
        clientSlots = new SemaphoreSlim(maxConcurrentClients, maxConcurrentClients);
    }

    public int Port => started
        ? ((IPEndPoint)listener.LocalEndpoint).Port
        : throw new InvalidOperationException("Server has not started.");

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        if (started)
        {
            throw new InvalidOperationException("Server can only be started once.");
        }

        listener.Start(maxConcurrentClients);
        started = true;
        Console.WriteLine($"Arena leaderboard listening on 127.0.0.1:{Port} protocol=v{WireProtocol.Version}");

        var activeClients = new HashSet<Task>();
        try
        {
            while (true)
            {
                await clientSlots.WaitAsync(cancellationToken);
                TcpClient client;
                try
                {
                    client = await listener.AcceptTcpClientAsync(cancellationToken);
                }
                catch
                {
                    clientSlots.Release();
                    throw;
                }

                activeClients.RemoveWhere(static task => task.IsCompleted);
                activeClients.Add(HandleClientAndReleaseAsync(client, cancellationToken));
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        finally
        {
            listener.Stop();
            await Task.WhenAll(activeClients);
            clientSlots.Dispose();
            Console.WriteLine("Arena leaderboard stopped.");
        }
    }

    private async Task HandleClientAndReleaseAsync(TcpClient client, CancellationToken serverToken)
    {
        try
        {
            await HandleClientAsync(client, serverToken);
        }
        finally
        {
            clientSlots.Release();
        }
    }

    private async Task HandleClientAsync(TcpClient client, CancellationToken serverToken)
    {
        using (client)
        using (var requestCancellation = CancellationTokenSource.CreateLinkedTokenSource(serverToken))
        {
            requestCancellation.CancelAfter(requestTimeout);
            NetworkStream stream = client.GetStream();
            try
            {
                byte[] payload = await WireProtocol.ReadFrameAsync(stream, requestCancellation.Token);
                ArenaRequest request = WireProtocol.ParseRequest(payload);
                byte[] response = Dispatch(request);
                await WireProtocol.WriteFrameAsync(stream, response, requestCancellation.Token);
            }
            catch (ProtocolException exception)
            {
                Console.Error.WriteLine($"request_rejected code={exception.Code}");
                await TryWriteErrorAsync(stream, exception.Code, serverToken);
            }
            catch (OperationCanceledException) when (serverToken.IsCancellationRequested)
            {
            }
            catch (OperationCanceledException) when (!serverToken.IsCancellationRequested)
            {
                Console.Error.WriteLine("request_rejected code=request_timeout");
            }
            catch (IOException)
            {
                Console.Error.WriteLine("request_rejected code=connection_io_error");
            }
            catch (SocketException)
            {
                Console.Error.WriteLine("request_rejected code=socket_error");
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine($"request_rejected code=internal_error exception={exception.GetType().Name}");
                await TryWriteErrorAsync(stream, "internal_error", serverToken);
            }
        }
    }

    private byte[] Dispatch(ArenaRequest request)
    {
        object response;
        switch (request.Kind)
        {
            case RequestKind.Health:
                response = new { version = WireProtocol.Version, ok = true, type = "health" };
                break;
            case RequestKind.SubmitScore:
                int bestScore = store.SubmitScore(request.PlayerId!, request.Score);
                response = new { version = WireProtocol.Version, ok = true, bestScore };
                break;
            case RequestKind.GetLeaderboard:
                ScoreEntry[] entries = store.GetTop(request.Limit);
                response = new { version = WireProtocol.Version, ok = true, entries };
                break;
            default:
                throw new ProtocolException("unsupported_type");
        }

        return WireProtocol.Serialize(response);
    }

    private static async Task TryWriteErrorAsync(
        NetworkStream stream,
        string errorCode,
        CancellationToken serverToken)
    {
        using var responseCancellation = CancellationTokenSource.CreateLinkedTokenSource(serverToken);
        responseCancellation.CancelAfter(TimeSpan.FromSeconds(1));
        try
        {
            byte[] response = WireProtocol.Serialize(new
            {
                version = WireProtocol.Version,
                ok = false,
                error = errorCode
            });
            await WireProtocol.WriteFrameAsync(stream, response, responseCancellation.Token);
        }
        catch (Exception exception) when (exception is IOException or SocketException or OperationCanceledException)
        {
        }
    }
}
