using System.Threading.Tasks;
using DDBMSP.Interfaces;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Core;
using Orleans;

namespace DDBMSP.Grains.Core
{
    public class StatefulGrain<TState, TSummary> : Grain<TState>, IStateful<TState, TSummary> where TState : ISummarizableTo<TSummary>, new()
    {
        public Task SetState(TState state, bool persist = true)
        {
            State = state;
            return persist ? WriteStateAsync() : Task.CompletedTask;
        }

        public Task<TSummary> Summarize() => State.Summarize();
    }
    
    public class StatefulGrain<TPod> : Grain<TPod>, IStateful<TPod> where TPod : class, new()
    {
        public Task SetState(TPod state, bool persist = true)
        {
            State = state;
            return persist ? WriteStateAsync() : Task.CompletedTask;
        }
    }
   
}