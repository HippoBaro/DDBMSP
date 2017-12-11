using System.Threading.Tasks;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Aggregators
{
    public interface IAggregator<TAggregated>
    {
        Task Aggregate(Immutable<TAggregated> grain);
    }
}