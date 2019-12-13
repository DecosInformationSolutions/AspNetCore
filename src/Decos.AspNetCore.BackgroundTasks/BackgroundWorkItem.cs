using System;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.AspNetCore.BackgroundTasks
{
    /// <summary>
    /// Provides a method for executing asynchronous tasks in the background.
    /// </summary>
    public static class BackgroundWorkItem
    {
        /// <summary>
        /// Schedules a task which can run in the background, independent of any
        /// request.
        /// </summary>
        /// <param name="queue">The queue on which to schedule the task.</param>
        /// <param name="method">The method to be run in the background.</param>
        public static void QueueBackgroundWorkItem(this IBackgroundTaskQueue queue, Func<CancellationToken, Task> method)
        {
            queue.Queue(new WorkOrder(method));
        }

        /// <summary>
        /// Represents a method to be executed as background work.
        /// </summary>
        public class WorkOrder : IBackgroundWorkOrder<WorkOrder, Worker>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="WorkOrder"/> class
            /// with the specified method.
            /// </summary>
            /// <param name="method">
            /// The method to be run in the background.
            /// </param>
            public WorkOrder(Func<CancellationToken, Task> method)
            {
                Method = method;
            }

            /// <summary>
            /// Gets the method to be run in the background.
            /// </summary>
            public Func<CancellationToken, Task> Method { get; }
        }

        /// <summary>
        /// Represents a worker that executes a <see cref="WorkOrder"/>.
        /// </summary>
        public class Worker : IBackgroundWorker<WorkOrder, Worker>
        {
            /// <summary>
            /// Executes a work order.
            /// </summary>
            /// <param name="order">The work order to execute.</param>
            /// <param name="cancellationToken">
            /// A token to monitor for cancellation requests.
            /// </param>
            /// <returns>
            /// A task that represents the asynchronous operation.
            /// </returns>
            public async Task DoWorkAsync(WorkOrder order, CancellationToken cancellationToken)
            {
                await order.Method.Invoke(cancellationToken);
            }
        }
    }
}