using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.Grains.Core
{
    public interface IResource<in TState, TData, TSummary> : IStateful<TState, TData, TSummary> where TState : IDataOf<TData>, ISummarizableTo<TSummary>, IExist
    {
        Task Create();
        Task<bool> Exits();
    }
    
    public interface IResource<in TState, TData> : IStateful<TState, TData> where TState : IDataOf<TData>, IExist
    {
        Task Create();
        Task<bool> Exits();
    }
}