using MuPegaso.GameServer;
using MuPegaso.Network;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine(@"
╔════════════════════════════════════════╗
║    MU PEGASO GAME SERVER  v0.1.0                    ║
║    Puerto: 55901  |  Tick: 20/s                     ║
╚════════════════════════════════════════╝");
Console.ResetColor();

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((ctx, cfg) => cfg
        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
        .WriteTo.File("logs/gameserver-.log", rollingInterval: RollingInterval.Day))
    .ConfigureServices(services =>
    {
        services.AddSingleton<NetworkServer>();
        services.AddHostedService<GameServerWorker>();
    })
    .Build();

await host.RunAsync();
