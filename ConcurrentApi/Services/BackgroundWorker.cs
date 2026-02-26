using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace ConcurrentApi.Services
{
    public class BackgroundWorker : BackgroundService
    {
        private readonly WorkQueue _queue;
        private readonly ProcessingController _controller;

        public BackgroundWorker(WorkQueue queue, ProcessingController controller)
        {
            Console.WriteLine("BackgroundWorker constructor called");
            _queue = queue;
            _controller = controller;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (true)
            {
                //Always respect app shutdown
                stoppingToken.ThrowIfCancellationRequested();

                //Stop here BEFORE reading
                await _controller.WaitIfStoppedAsync(stoppingToken);

                //Only read when allowed
                var item = await _queue.Reader.ReadAsync(stoppingToken);

                Console.WriteLine("Processing item...");
                await Task.Delay(500, stoppingToken); // simulate work
                _queue.IncrementProcessed();
            }
        }
    }
}
