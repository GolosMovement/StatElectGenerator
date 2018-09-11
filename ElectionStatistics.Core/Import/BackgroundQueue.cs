using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Import
{
    public class BackgroundQueue : IBackgroundQueue
    {
        private ConcurrentQueue<Func<CancellationToken, ModelContext, Task>> items =
            new ConcurrentQueue<Func<CancellationToken, ModelContext, Task>>();
        private SemaphoreSlim signal = new SemaphoreSlim(0);
        public void Enqueue(Func<CancellationToken, ModelContext, Task> item)
        {
            items.Enqueue(item);
            signal.Release();
        }

        public async Task<Func<CancellationToken, ModelContext, Task>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await signal.WaitAsync(cancellationToken);
            items.TryDequeue(out var item);

            return item;
        }
    }
}