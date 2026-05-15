namespace MuPegaso.Core.Interfaces
{
    public interface IGameService
    {
        Task StartAsync(CancellationToken ct);
        Task StopAsync(CancellationToken ct);
    }
}
