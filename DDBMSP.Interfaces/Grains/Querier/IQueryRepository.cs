using System.Threading.Tasks;
using DDBMSP.Entities.Query;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Querier
{
    public interface IQueryRepository : IGrainWithIntegerKey
    {
        Task CommitQuery(Immutable<QueryDefinition> queryDefinition);
        Task<Immutable<QueryDefinition>> GetQueryDefinition(Immutable<string> name);
    }
}