using System;

using Decos.AspNetCore.BackgroundTasks;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides a set of static methods for adding background work support.
    /// </summary>
    public static class BackgroundTasksServiceCollectionExtensions
    {
        /// <summary>
        /// Adds services for queueing background work items.
        /// </summary>
        /// <param name="services">
        /// The service collection to add the services to.
        /// </param>
        /// <returns>A reference to the service collection.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="services"/> is <c>null</c>.
        /// </exception>
        public static IServiceCollection AddBackgroundTasks(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // Provide a single queue for all background work in the application
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            // Register the worker that executes generic work orders
            services.AddScoped<BackgroundWorkItem.Worker>();

            // Register the service that processes the background task queue
            services.AddHostedService<QueuedHostedService>();

            return services;
        }
    }
}