using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Aggregators
{
    public interface IAggregator<TAggregated>
    {
        Task Aggregate(TAggregated grain);
        Task AggregateRange(List<TAggregated> articles);
    }
    
    public interface IAggregator<in TAggregated1, TAggregated2>
    {
        Task Aggregate(TAggregated1 first, TAggregated2 second);
        Task AggregateRange(TAggregated1 first, List<TAggregated2> articles);
    }
    
    public interface IGlobalAggregator<TAggregated>
    {
        Task Aggregate(Immutable<TAggregated> grain);
        Task AggregateRange(Immutable<List<TAggregated>> articles);
    }
    
    public interface IGlobalAggregator<TAggregated1, TAggregated2>
    {
        Task Aggregate(Immutable<TAggregated1> first, Immutable<TAggregated2> second);
        Task AggregateRange(Immutable<TAggregated1> first, Immutable<List<TAggregated2>> articles);
    }
}