using System;
using System.Threading;
using System.Threading.Tasks;

using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Import
{
    public interface IBackgroundQueue
    {
        void Enqueue(Func<CancellationToken, ModelContext, Task> item);
        Task<Func<CancellationToken, ModelContext, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }
}