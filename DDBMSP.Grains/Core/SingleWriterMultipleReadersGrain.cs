using Orleans;

namespace DDBMSP.Grains.Core
{
    public class SingleWriterMultipleReadersGrain : Grain
    {
        protected AsyncSerialExecutor SerialExecutor { get; } = new AsyncSerialExecutor();
    }
    
    public class SingleWriterMultipleReadersGrain<TState> : ScheduledPersistedGrain<TState> where TState : new()
    {
        protected AsyncSerialExecutor SerialExecutor { get; } = new AsyncSerialExecutor();
    }
}
