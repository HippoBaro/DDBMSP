using System.Threading.Tasks;
using DDBMSP.Entities.Query;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Querier
{
    public interface IGenericQuerier : IGrainWithIntegerKey
    {
        Task CommitQuery(Immutable<QueryDefinition> queryDefinition);
        Task<Immutable<dynamic>> Query(Immutable<string> queryName);
    }
}