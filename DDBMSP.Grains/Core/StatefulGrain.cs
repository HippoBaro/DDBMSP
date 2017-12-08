using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Interfaces;
using Orleans;

namespace DDBMSP.Grains.Core
{
    public class StatefulGrain<TPod> : Grain<TPod>, IStateContainer<TPod> where TPod : class, new()
    {
        public Task<TPod> GetState()
        {
            return Task.FromResult(State);
        }

        public Task SetState(TPod state, bool persist = true)
        {
            State = state;
            return persist ? WriteStateAsync() : Task.CompletedTask;
        }
    }
}