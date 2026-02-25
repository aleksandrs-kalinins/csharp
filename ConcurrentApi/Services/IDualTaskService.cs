namespace ConcurrentApi.Services
{
    public interface IDualTaskService
    {
        void StartWork(CancellationToken stoppingToken);
        Task StopWorkAsync();
    }
}