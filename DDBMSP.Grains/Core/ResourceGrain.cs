using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Grains.Core
{
    public class ResourceGrain<TState, TSummary> : StatefulGrain<TState, TSummary>, IResource<TState, TSummary> where TState : class, ISummarizableTo<TSummary>, IExist, new()
    {
        public Task Create()
        {
            State.Exists = true;
            return Task.CompletedTask;
        }

        public Task<bool> Exits() => Task.FromResult(State.Exists);
    }
    
    public class ResourceGrain<TState> : StatefulGrain<TState>, IResource<TState> where TState : class, IExist, new()
    {
        public Task Create()
        {
            State.Exists = true;
            return Task.CompletedTask;
        }

        public Task<bool> Exits() => Task.FromResult(State.Exists);
    }
}