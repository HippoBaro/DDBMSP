using System.Threading.Tasks;
using DDBMSP.Entities.Query;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Querier
{
    public interface IDynamicQueryable
    {
        Task<Immutable<dynamic>> Execute(Immutable<QueryDefinition> query);
    }
}