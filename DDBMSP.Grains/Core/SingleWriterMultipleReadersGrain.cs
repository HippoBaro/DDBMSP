using Orleans;

namespace DDBMSP.Grains.Core
{
    public class SingleWriterMultipleReadersGrain : Grain
    {
        protected AsyncSerialExecutor SerialExecutor { get; } = new AsyncSerialExecutor();
    }
}