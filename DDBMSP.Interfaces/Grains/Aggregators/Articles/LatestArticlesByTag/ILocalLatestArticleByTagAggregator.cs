using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag
{
    public interface ILocalLatestArticleByTagAggregator: IGrainWithStringKey, IAggregator<ArticleSummary>
    {
        
    }
}