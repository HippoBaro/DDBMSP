using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles
{
    public interface IArticleAggregatorHubGrain : IGrainWithIntegerKey, IAggregator<ArticleSummary>
    {
    }
}