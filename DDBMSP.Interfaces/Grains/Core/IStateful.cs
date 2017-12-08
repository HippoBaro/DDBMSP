using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.Grains.Core
{
    public interface IStateful<in TState, TSummary> : IStateful<TState>, ISummarizableTo<TSummary> where TState : ISummarizableTo<TSummary>
    {
    }
    
    public interface IStateful<in TState>
    {
        Task SetState(TState state, bool persist = true);
    }
}