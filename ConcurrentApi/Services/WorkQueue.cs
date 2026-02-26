using System.Threading.Channels;

namespace ConcurrentApi.Services
{
    public class WorkQueue
    {
        private readonly Channel<int> _channel;
        private int _processed;

        public WorkQueue()
        {
            _channel = Channel.CreateBounded<int>(capacity: 100);
        }

        public async Task EnqueueAsync(CancellationToken ct)
        {
            await _channel.Writer.WriteAsync(1, ct);
        }

        public ChannelReader<int> Reader => _channel.Reader;

        public object GetStats() => new
        {
            Processed = _processed
        };

        public void IncrementProcessed()
            => Interlocked.Increment(ref _processed);
    }
}
