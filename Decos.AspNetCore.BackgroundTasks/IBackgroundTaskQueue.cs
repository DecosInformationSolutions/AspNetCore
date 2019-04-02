using System;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.AspNetCore.BackgroundTasks
{
    /// <summary>
    /// Defines a queue for scheduling background work.
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Schedules a work order which can run in the background.
        /// </summary>
        /// <typeparam name="TWorkOrder">
        /// The type of work order that <typeparamref name="TWorker"/> can
        /// execute.
        /// </typeparam>
        /// <typeparam name="TWorker">
        /// The type of worker that can execute a <typeparamref
        /// name="TWorkOrder"/>.
        /// </typeparam>
        /// <param name="order">The work order to schedule.</param>
        /// <exception cref="ArgumentNullException"><paramref name="order"/> is <c>null</c>.</exception>
        void Queue<TWorkOrder, TWorker>(IBackgroundWorkOrder<TWorkOrder, TWorker> order)
            where TWorkOrder : IBackgroundWorkOrder<TWorkOrder, TWorker>
            where TWorker : IBackgroundWorker<TWorkOrder, TWorker>;

        /// <summary>
        /// Returns a scheduled work order.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<IBackgroundWorkOrder> DequeueAsync(CancellationToken cancellationToken);
    }
}