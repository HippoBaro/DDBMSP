using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Core;
using Orleans;

namespace DDBMSP.Grains.Core
{
    public class StatefulGrain<TState, TData, TSummary> : Grain<TState>, IStateful<TState, TData, TSummary> where TState : IDataOf<TData>, IComposedBy<TState, TData>, ISummarizableTo<TSummary>, new()
    {
        public async Task SetState(TState state, bool persist = true)
        {
            State = state;
            if (persist)
                await WriteStateAsync();
        }

        public Task<TSummary> Summarize() => State.Summarize();
        public Task<TData> Data() => State.Data();
        
        public async Task Populate(TData component, bool persist = true)
        {
            State.Populate(component);
            if (persist)
                await  WriteStateAsync();
        }
    }
    
    public class StatefulGrain<TPod, TData> : Grain<TPod>, IStateful<TPod, TData> where TPod : class, TData, IDataOf<TData>, IComposedBy<TPod, TData>, new()
    {
        public async Task SetState(TPod state, bool persist = true)
        {
            State = state;
            if (persist)
                await WriteStateAsync();
        }
        public Task<TData> Data() => State.Data();
        
        public async Task Populate(TData component, bool persist = true)
        {
            State.Populate(component);
            if (persist)
                await  WriteStateAsync();
        }
    }
}