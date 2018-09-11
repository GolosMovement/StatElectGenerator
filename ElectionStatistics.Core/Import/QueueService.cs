using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

using ElectionStatistics.Model;

namespace ElectionStatistics.Core.Import
{
    public class QueueService : BackgroundService, IDisposable
    {
        private readonly IBackgroundQueue backgroundQueue;
        private readonly ILogger logger;
        private readonly IServiceProvider services;

        public QueueService(IBackgroundQueue backgroundQueue,
            ILoggerFactory loggerFactory, IServiceProvider services)
        {
            this.backgroundQueue = backgroundQueue;
            this.logger = loggerFactory.CreateLogger<QueueService>();
            this.services = services;
        }

        protected override async Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var item = await backgroundQueue.DequeueAsync(
                    cancellationToken);

                try
                {
                    using (var scope = services.CreateScope())
                    {
                        using (var dbc = scope.ServiceProvider
                            .GetRequiredService<ModelContext>())
                        {
                            await item(cancellationToken, dbc);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error occured in a background process");
                }
            }
        }
    }
}
