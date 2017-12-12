using System.Threading.Tasks;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Aggregators
{
    public interface IAggregator<TAggregated>
    {
        Task Aggregate(Immutable<TAggregated> grain);
    }
    
    public interface IAggregator<TAggregated1, TAggregated2>
    {
        Task Aggregate(Immutable<TAggregated1> first, Immutable<TAggregated2> second);
    }
}