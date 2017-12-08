using System.Threading.Tasks;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles
{
    public interface IArticleAggregatorHubGrain : IGrainWithIntegerKey
    {
        Task Aggregate(IArticle article);
    }
}