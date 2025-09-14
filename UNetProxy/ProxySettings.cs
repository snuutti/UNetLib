using System.ComponentModel;
using Spectre.Console.Cli;
using UNetLib.LLAPI;

namespace UNetProxy;

public class ProxySettings : CommandSettings
{
    [Description("Port to listen on for incoming UDP packets.")]
    [CommandArgument(0, "<Listen Port>")]
    public required int ListenPort { get; set; }

    [Description("Target host to forward packets to.")]
    [CommandArgument(1, "<Target Host>")]
    public required string TargetHost { get; set; }

    [Description("Target port to forward packets to.")]
    [CommandArgument(2, "<Target Port>")]
    public required int TargetPort { get; set; }

    [Description("Use long acks.")]
    [CommandOption("--long-acks")]
    [DefaultValue(false)]
    public bool LongAcks { get; set; }

    [Description("Channel types.")]
    [CommandOption("-c|--channel <VALUES>")]
    public QosType[] Channels { get; set; } = [
        QosType.ReliableSequenced,
        QosType.Unreliable
    ];

    [Description("Enable logging of ping packets.")]
    [CommandOption("--log-pings")]
    [DefaultValue(true)]
    public bool LogPings { get; set; }

    [Description("Enable logging of ack packets.")]
    [CommandOption("--log-acks")]
    [DefaultValue(true)]
    public bool LogAcks { get; set; }
}