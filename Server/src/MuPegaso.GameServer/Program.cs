using MuPegaso.GameServer;
using MuPegaso.Network;
using Serilog;

// ── Banner MU PEGASO ─────────────────────────────────────
Console.OutputEncoding = System.Text.Encoding.UTF8;
string[] logo = {
    "███    ███  ██       ██     ██████  ███████   ██████   █████   ███████  ██████ ",
    "████  ████  ██       ██     ██   ██ ██       ██       ██   ██  ██      ██    ██",
    "██ ████ ██  ██       ██     ██████  █████    ██   ███ ███████  ███████ ██    ██",
    "██  ██  ██  ██       ██     ██      ██       ██    ██ ██   ██       ██ ██    ██",
    "██      ██  ███████  ██     ██      ███████   ██████  ██   ██  ███████  ██████ "
};
int ancho = 88;
string lineaBanner = new string('─', ancho);
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine($"\n┌{lineaBanner}┐");
foreach (var l in logo)
{
    int espacios = (ancho - l.Length) / 2;
    string pad = new string(' ', espacios);
    string padDer = new string(' ', ancho - l.Length - espacios);
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write("│");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write($"{pad}{l}{padDer}");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("│");
}
Console.WriteLine($"│{new string(' ', ancho)}│");
string version = "Mu Pegaso v0.1.0";
string url = "https://mupegaso.com";
Console.Write("│");
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Write(version.PadLeft((ancho + version.Length) / 2).PadRight(ancho));
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine("│");
Console.Write("│");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.Write(url.PadLeft((ancho + url.Length) / 2).PadRight(ancho));
Console.WriteLine("│");
Console.WriteLine($"│{new string(' ', ancho)}│");
Console.Write("│    💻 ");
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Write("Server:      ");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.Write($"{"mu-pegaso-server, 0.1.0",-63}");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine(" │");
Console.Write("│    🚀 ");
Console.ForegroundColor = ConsoleColor.Cyan;
Console.Write("Deploy:      ");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.Write($"{"https://mupegaso.com",-63}");
Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine(" │");
Console.WriteLine($"└{lineaBanner}┘\n");
Console.ResetColor();
// ─────────────────────────────────────────────────────────

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

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
