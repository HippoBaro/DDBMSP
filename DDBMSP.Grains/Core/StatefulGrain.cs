using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Core;
using Orleans;

namespace DDBMSP.Grains.Core
{
    public class StatefulGrain<TState, TData, TSummary> : Grain<TState>, IStateful<TState, TData, TSummary> where TState : IDataOf<TData>, ISummarizableTo<TSummary>, new()
    {
        public Task SetState(TState state, bool persist = true)
        {
            State = state;
            return persist ? WriteStateAsync() : Task.CompletedTask;
        }

        public Task<TSummary> Summarize() => State.Summarize();
        public Task<TData> Data() => State.Data();
    }
    
    public class StatefulGrain<TPod, TData> : Grain<TPod>, IStateful<TPod, TData> where TPod : class, IDataOf<TData>, new()
    {
        public Task SetState(TPod state, bool persist = true)
        {
            State = state;
            return persist ? WriteStateAsync() : Task.CompletedTask;
        }
        public Task<TData> Data() => State.Data();
    }
   
}