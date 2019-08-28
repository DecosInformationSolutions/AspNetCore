namespace Decos.AspNetCore.BackgroundTasks
{
    /// <summary>
    /// Defines an object that represents background work that needs to be done.
    /// </summary>
    /// <typeparam name="TWorkOrder">
    /// The type of work order that <typeparamref name="TWorker"/> can execute.
    /// </typeparam>
    /// <typeparam name="TWorker">
    /// The type of worker that can execute a <typeparamref name="TWorkOrder"/>.
    /// </typeparam>
    public interface IBackgroundWorkOrder<TWorkOrder, TWorker> : IBackgroundWorkOrder
        where TWorkOrder : IBackgroundWorkOrder<TWorkOrder, TWorker>
        where TWorker : IBackgroundWorker<TWorkOrder, TWorker>
    {
    }
}