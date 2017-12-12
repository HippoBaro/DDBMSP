using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles
{
    public interface ILocalLatestArticleAggregator : IGrainWithIntegerKey, IAggregator<ArticleSummary>
    {
        
    }
}