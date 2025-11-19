using NetworkStarterSample;
using UNetLib;
using UNetLib.LLAPI;

var config = new ConnectionConfig
{
    Channels = [
        QosType.ReliableSequenced,
        QosType.Unreliable
    ]
};

var listener = new MyEventListener();
var server = new UNetServer(config, listener);
server.Start(7777);

_ = Task.Run(async () =>
{
    while (server.IsRunning)
    {
        server.Update();
        await Task.Delay(10);
    }
});

Console.WriteLine("Server started. Press Enter to stop.");
Console.ReadLine();

server.Stop();