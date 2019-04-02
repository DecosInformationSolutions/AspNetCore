using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Decos.AspNetCore.BackgroundTasks.Tests
{
    [TestClass]
    public class QueueHostedServiceTests
    {
        [TestMethod]
        public async Task HostedServiceShouldExecuteBackgroundWorkWhileRunning()
        {
            await RunHostedServiceAsync(async (_, taskQueue) =>
            {
                var isExecuted = false;
                taskQueue.QueueBackgroundWorkItem(__ =>
                {
                    isExecuted = true;
                    return Task.CompletedTask;
                });

                await Task.Delay(10);
                Assert.IsTrue(isExecuted);
            });
        }

        [TestMethod]
        public void ServiceCollectionExtensionsShouldProvideRequiredServices()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddBackgroundTasks()
                .BuildServiceProvider();

            serviceProvider.GetRequiredService<IBackgroundTaskQueue>();
            serviceProvider.GetRequiredService<BackgroundWorkItem.Worker>();
            serviceProvider.GetServices<IHostedService>()
                .ShouldContain<QueuedHostedService>();
        }

        [TestMethod]
        public async Task HostedServiceShouldCancelTasksWhenStopping()
        {
            await RunHostedServiceAsync(async (service, taskQueue) =>
            {
                var isCanceled = false;
                taskQueue.QueueBackgroundWorkItem(async token =>
                {
                    try
                    {
                        await Task.Delay(-1, token);
                    }
                    catch (OperationCanceledException) { }

                    isCanceled = true;
                });

                await Task.Delay(10);
                await service.StopAsync(default);

                Assert.IsTrue(isCanceled);
            });
        }

        [TestMethod]
        public async Task HostedServiceShouldExitWhenForcingShutdown()
        {
            await RunHostedServiceAsync(async (service, taskQueue) =>
            {
                taskQueue.QueueBackgroundWorkItem(async _ =>
                {
                    await Task.Delay(-1);
                });

                await Task.Delay(10);
                var cancellationTokenSource = new CancellationTokenSource(10);
                await service.StopAsync(cancellationTokenSource.Token);
            });
        }

        private async Task RunHostedServiceAsync(Func<QueuedHostedService, IBackgroundTaskQueue, Task> test)
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging(config => config.AddConsole())
                .AddScoped<BackgroundWorkItem.Worker>()
                .BuildServiceProvider();
            var taskQueue = new BackgroundTaskQueue();
            var service = new QueuedHostedService(
                serviceProvider,
                taskQueue,
                serviceProvider.GetRequiredService<ILoggerFactory>());
            await service.StartAsync(default);

            try
            {
                await test(service, taskQueue);
            }
            finally
            {
                var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                await service.StopAsync(cancellationTokenSource.Token);
            }
        }
    }
}