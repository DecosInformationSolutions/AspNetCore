using System;
using System.Threading;
using System.Threading.Tasks;

namespace Decos.AspNetCore.BackgroundTasks
{
    /// <summary>
    /// Defines a method that executes background work orders.
    /// </summary>
    /// <typeparam name="TWorkOrder">
    /// The type of work order that <typeparamref name="TWorker"/> can execute.
    /// </typeparam>
    /// <typeparam name="TWorker">
    /// The type of worker that can execute a <typeparamref name="TWorkOrder"/>.
    /// </typeparam>
    public interface IBackgroundWorker<TWorkOrder, TWorker> : IBackgroundWorker
        where TWorkOrder : IBackgroundWorkOrder<TWorkOrder, TWorker>
        where TWorker : IBackgroundWorker<TWorkOrder, TWorker>
    {
        /// <summary>
        /// Executes a work order.
        /// </summary>
        /// <param name="order">The work order to execute.</param>
        /// <param name="cancellationToken">
        /// A token to monitor for cancellation requests.
        /// </param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task DoWorkAsync(TWorkOrder order, CancellationToken cancellationToken);
    }
}