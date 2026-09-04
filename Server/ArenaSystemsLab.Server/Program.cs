using ArenaSystemsLab.Server;

if (!TryReadPort(args, out int port))
{
    Console.Error.WriteLine("Usage: ArenaSystemsLab.Server [--port 1-65535]");
    return 2;
}

using var shutdown = new CancellationTokenSource();
Console.CancelKeyPress += (_, eventArgs) =>
{
    eventArgs.Cancel = true;
    shutdown.Cancel();
};

var server = new LeaderboardServer(port);
await server.RunAsync(shutdown.Token);
return 0;

static bool TryReadPort(string[] arguments, out int port)
{
    port = 7777;
    if (arguments.Length == 0)
    {
        return true;
    }

    return arguments.Length == 2
        && arguments[0] == "--port"
        && int.TryParse(arguments[1], out port)
        && port is >= 1 and <= 65535;
}
