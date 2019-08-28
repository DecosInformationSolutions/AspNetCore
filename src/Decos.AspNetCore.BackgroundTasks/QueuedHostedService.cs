using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Decos.AspNetCore.BackgroundTasks
{
    /// <summary>
    /// Executes background work scheduled in a <see
    /// cref="IBackgroundTaskQueue"/>.
    /// </summary>
    public class QueuedHostedService : IHostedService
    {
        private readonly IServiceProvider _services;
        private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();
        private readonly ILogger _logger;
        private Task _backgroundTask;

        /// <summary>
        /// Initializes a new instance of the <see cref="QueuedHostedService"/>
        /// class with the specified service provider, background task queue and
        /// logger factory.
        /// </summary>
        /// <param name="services">
        /// Used to retrieve instances of background workers.
        /// </param>
        /// <param name="taskQueue">
        /// A queue used to schedule background work.
        /// </param>
        /// <param name="loggerFactory">
        /// Used to construct logger instances.
        /// </param>
        public QueuedHostedService(
            IServiceProvider services,
            IBackgroundTaskQueue taskQueue,
            ILoggerFactory loggerFactory)
        {
            _services = services;
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }

        /// <summary>
        /// Gets the queue used to schedule background work.
        /// </summary>
        public IBackgroundTaskQueue TaskQueue { get; }

        /// <summary>
        /// Triggered when the application host is ready to start the service.
        /// </summary>
        /// <param name="cancellationToken">
        /// Indicates that the start process has been aborted.
        /// </param>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is starting.");

            _backgroundTask = Task.Run(BackgroundProcessing);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Triggered when the application host is performing a graceful
        /// shutdown.
        /// </summary>
        /// <param name="cancellationToken">
        /// Indicates that the shutdown process should no longer be graceful.
        /// </param>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            if (_shutdown.IsCancellationRequested)
                return Task.CompletedTask;

            _logger.LogInformation("Queued Hosted Service is stopping.");

            _shutdown.Cancel();

            return Task.WhenAny(
                _backgroundTask,
                Task.Delay(Timeout.Infinite, cancellationToken));
        }

        private async Task BackgroundProcessing()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                var workOrder = await TaskQueue.DequeueAsync(_shutdown.Token);

                try
                {
                    using (var scope = _services.CreateScope())
                    {
                        var workerType = workOrder
                            .GetType()
                            .GetInterfaces()
                            .First(t => t.IsConstructedGenericType && t.GetGenericTypeDefinition() == typeof(IBackgroundWorkOrder<,>))
                            .GetGenericArguments()
                            .Last();

                        var worker = scope.ServiceProvider
                            .GetRequiredService(workerType);

                        var task = (Task)workerType
                            .GetMethod("DoWorkAsync")
                            .Invoke(worker, new object[] { workOrder, _shutdown.Token });
                        await task;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"Error occurred executing {nameof(workOrder)}.");
                }
            }
        }
    }
}