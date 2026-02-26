using Microsoft.AspNetCore.Mvc;

namespace ConcurrentApi.Services
{
    public class ProcessingController
    {
        private readonly SemaphoreSlim _signal = new(0, 1);
        private volatile bool _running = true;

        

        public async Task WaitIfStoppedAsync(CancellationToken ct)
        {
            if (_running) return;
            await _signal.WaitAsync(ct);
        }

        public void Stop()
        {
            _running = false;
        }

        public void Start()
        {
            if (_running) return;
            _running = true;
            _signal.Release();
        }

        //might be usefule for debugging or UI
        public bool IsRunning => _running;
    }
}
