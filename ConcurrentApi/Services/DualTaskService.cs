using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConcurrentApi.Services;

public class DualTaskService : BackgroundService
{
    private readonly ILogger<DualTaskService> _logger;

    public DualTaskService(ILogger<DualTaskService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.Register(() =>
        Console.WriteLine(">>> Cancellation token triggered <<<"));

        Console.WriteLine(">>> DualTaskService STARTED <<<");

        try
        {

            var task1 = RunTask1Async(stoppingToken);
            var task2 = RunTask2Async(stoppingToken);

            await Task.WhenAll(task1, task2);
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine(">>> Task cancelled <<<");
        }
    }

    private async Task RunTask1Async(CancellationToken token)
    {
        Console.WriteLine("Task 1 starting");
        while (!token.IsCancellationRequested)
        {
            _logger.LogInformation("Task 1 running");
            await Task.Delay(2000, token);
        }
        Console.WriteLine("Task 1 finished");
    }

    private async Task RunTask2Async(CancellationToken token)
    {
        Console.WriteLine("Task 2 starting");
        while (!token.IsCancellationRequested)
        {
            _logger.LogInformation("Task 2 running");
            await Task.Delay(3000, token);
        }
        Console.WriteLine("Task 2 finished");

    }
}
