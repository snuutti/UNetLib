using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Spectre.Console;
using Spectre.Console.Cli;

namespace UNetProxy;

public class ProxyCommand : AsyncCommand<ProxySettings>
{
    private readonly ConcurrentDictionary<IPEndPoint, UdpSession> _sessions = new();

    public override async Task<int> ExecuteAsync(CommandContext context, ProxySettings settings)
    {
        AnsiConsole.WriteLine("UNetProxy");
        AnsiConsole.WriteLine();

        var targetIps = await Dns.GetHostAddressesAsync(settings.TargetHost, AddressFamily.InterNetwork);
        if (targetIps.Length == 0)
        {
            AnsiConsole.MarkupLine($"[red]Could not resolve host {settings.TargetHost}[/]");
            return 1;
        }

        var targetEndPoint = new IPEndPoint(targetIps[0], settings.TargetPort);

        using var listener = new UdpClient(settings.ListenPort);
        using var sender = new UdpClient();

        AnsiConsole.WriteLine($"Listening on port {settings.ListenPort}, forwarding to {targetEndPoint}.");

        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            cts.Cancel();
        };

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var result = await listener.ReceiveAsync(ct);
                var sourceEndPoint = result.RemoteEndPoint;
                var buffer = result.Buffer;

                if (!_sessions.TryGetValue(sourceEndPoint, out var session))
                {
                    // TODO: Cleanup old sessions
                    session = new UdpSession(sourceEndPoint, targetEndPoint, listener, cts);
                    _sessions[sourceEndPoint] = session;
                    AnsiConsole.MarkupLine($"[blue]Created new session for {sourceEndPoint}[/]");
                }

                await session.OnPacketReceived(buffer, true);
            }
            catch (OperationCanceledException)
            {
                AnsiConsole.MarkupLine("[yellow]Shutting down...[/]");
            }
            catch (Exception e)
            {
                AnsiConsole.MarkupLine($"[red]{e}[/]");
            }
        }

        return 0;
    }
}