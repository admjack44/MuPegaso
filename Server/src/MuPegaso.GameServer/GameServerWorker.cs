using MuPegaso.Network;

namespace MuPegaso.GameServer
{
    public class GameServerWorker : BackgroundService
    {
        private readonly NetworkServer _network;
        private readonly ILogger<GameServerWorker> _logger;
        private DateTime _lastTick = DateTime.UtcNow;

        public GameServerWorker(NetworkServer network, ILogger<GameServerWorker> logger)
        {
            _network = network;
            _logger  = logger;
        }

        public override Task StartAsync(CancellationToken ct)
        {
            _network.Start(55901);
            _logger.LogInformation("Servidor listo. Esperando conexiones...");
            return base.StartAsync(ct);
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            const int tickMs = 1000 / 20;
            while (!ct.IsCancellationRequested)
            {
                var now   = DateTime.UtcNow;
                float dt  = (float)(now - _lastTick).TotalSeconds;
                _lastTick = now;
                // World.Update(dt) — Fase 3
                await Task.Delay(tickMs, ct).ConfigureAwait(false);
            }
        }

        public override async Task StopAsync(CancellationToken ct)
        {
            _logger.LogInformation("Apagando servidor...");
            _network.Dispose();
            await base.StopAsync(ct);
        }
    }
}
