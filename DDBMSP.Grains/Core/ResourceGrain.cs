using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Core;
using Orleans;

namespace DDBMSP.Grains.Core
{
    public class ResourceGrain<TState, TData, TSummary> : StatefulGrain<TState, TData, TSummary>, IResource<TState, TData, TSummary> where TState : IDataOf<TData>, ISummarizableTo<TSummary>, IExist, IComposedBy<TState, TData>, new()
    {
        public Task Create()
        {
            State.Exists = true;
            State.Id = this.GetPrimaryKey();
            return Task.CompletedTask;
        }

        public Task<bool> Exits() => Task.FromResult(State.Exists);
    }
    
    public class ResourceGrain<TState, TData> : StatefulGrain<TState, TData>, IResource<TState, TData> where TState : class, TData, IDataOf<TData>, IExist, IComposedBy<TState, TData>, new()
    {
        public Task Create()
        {
            State.Exists = true;
            State.Id = this.GetPrimaryKey();
            return Task.CompletedTask;
        }

        public Task<bool> Exits() => Task.FromResult(State.Exists);
    }
}