using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.Grains.Core
{
    public interface IStateful<in TState, TData, TSummary> : IStateful<TState, TData>, ISummarizableTo<TSummary> where TState : IDataOf<TData>, ISummarizableTo<TSummary>
    {
    }
    
    public interface IStateful<in TState, TData> :  IDataOf<TData> where TState : IDataOf<TData>
    {
        Task SetState(TState state, bool persist = true);
    }
}