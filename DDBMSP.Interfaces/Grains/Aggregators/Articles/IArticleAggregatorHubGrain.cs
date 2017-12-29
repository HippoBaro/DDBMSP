using DDBMSP.Entities.Article;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles
{   
    public interface IArticleAggregatorHubGrain : IGrainWithIntegerKey, IAggregator<ArticleState>
    {
    }
}