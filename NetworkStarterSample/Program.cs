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

Console.WriteLine("Server started. Press Enter to stop.");
Console.ReadLine();

server.Stop();