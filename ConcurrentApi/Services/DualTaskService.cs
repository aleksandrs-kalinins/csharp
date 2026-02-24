using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ConcurrentApi.Services;

public class DualTaskService : BackgroundService
{
    private readonly ILogger<DualTaskService> _logger;
    private CancellationTokenSource? _cts;
    private Task? _runningTask;

    public DualTaskService(ILogger<DualTaskService> logger)
    {
        _logger = logger;
        Console.WriteLine("DualTaskService CONSTRUCTOR");
    }

    public void StartWork(CancellationToken stoppingToken)
    {
        if (_runningTask != null && !_runningTask.IsCompleted)
        {
            _logger.LogInformation("Tasks already running");
            return;
        }

        _logger.LogInformation("Starting tasks");

        _cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
        _runningTask = RunAsync(_cts.Token);
    }

    private async Task RunAsync(CancellationToken token)
    {
        var t1 = Task1Async(token);
        var t2 = Task2Async(token);

        await Task.WhenAll(t1, t2);
    }

    private async Task Task1Async(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Console.WriteLine("Task 1 running");
            await Task.Delay(2000, token);
        }
    }

    private async Task Task2Async(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Console.WriteLine("Task 2 running");
            await Task.Delay(3000, token);
        }
    }

    public async Task StopWorkAsync()
    {
        if (_cts == null)
            return;

        _logger.LogInformation("Stopping tasks");

        _cts.Cancel();

        try
        {
            if (_runningTask != null)
                await _runningTask;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Tasks cancelled");
        }
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("Starting ExecuteAsync...");
        StartWork(stoppingToken);
        return Task.CompletedTask;

    }
}
