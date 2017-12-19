using DDBMSP.Entities.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles
{   
    public interface IArticleAggregatorHubGrain : IGrainWithIntegerKey, IAggregator<ArticleSummary>
    {
    }
}