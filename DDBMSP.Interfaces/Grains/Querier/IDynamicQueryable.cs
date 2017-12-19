using System.Threading.Tasks;
using DDBMSP.Entities.Query;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Querier
{
    public interface IDynamicQueryable
    {
        Task<Immutable<dynamic>> Query(Immutable<string> queryName);
    }
}