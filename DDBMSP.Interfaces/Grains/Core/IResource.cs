using System.Threading.Tasks;
using DDBMSP.Interfaces.Converters;
using DDBMSP.Interfaces.PODs.Core;
using Newtonsoft.Json;

namespace DDBMSP.Interfaces.Grains.Core
{
    public interface IResource<in TState, TSummary> : IStateful<TState, TSummary> where TState : ISummarizableTo<TSummary>, IExist
    {
        Task Create();
        Task<bool> Exits();
    }
    
    public interface IResource<in TState> : IStateful<TState> where TState : IExist
    {
        Task Create();
        Task<bool> Exits();
    }
}