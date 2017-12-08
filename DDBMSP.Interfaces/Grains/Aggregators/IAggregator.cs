using System.Threading.Tasks;

namespace DDBMSP.Interfaces.Grains.Aggregators
{
    public interface IAggregator<in TAggregated>
    {
        Task Aggregate(TAggregated grain);
    }
}