using Spectre.Console.Cli;
using UNetProxy;

var app = new CommandApp<ProxyCommand>();
return app.Run(args);