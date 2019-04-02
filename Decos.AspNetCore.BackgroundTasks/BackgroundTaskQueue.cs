using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.AspNetCore.BackgroundTasks
{
    /// <summary>
    /// Represents a queue for scheduling background work.
    /// </summary>
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<IBackgroundWorkOrder> _workOrders =
            new ConcurrentQueue<IBackgroundWorkOrder>();

        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

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
        /// <exception cref="ArgumentNullException">
        /// <paramref name="order"/> is <c>null</c>.
        /// </exception>
        public void Queue<TWorkOrder, TWorker>(IBackgroundWorkOrder<TWorkOrder, TWorker> order)
            where TWorkOrder : IBackgroundWorkOrder<TWorkOrder, TWorker>
            where TWorker : IBackgroundWorker<TWorkOrder, TWorker>
        {
            if (order == null)
            {
                throw new ArgumentNullException(nameof(order));
            }

            _workOrders.Enqueue(order);
            _signal.Release();
        }

        /// <summary>
        /// Returns a scheduled work order.
        /// </summary>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task<IBackgroundWorkOrder> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workOrders.TryDequeue(out var workItem);

            return workItem;
        }
    }
}