using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles.Search
{
    public interface ILocalSearchArticleAggregator : IGrainWithIntegerKey, IAggregator<ArticleSummary>
    {
        
    }
}