using DDBMSP.Entities.Article;
using DDBMSP.Entities.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles.Search
{
    public interface ILocalSearchArticleAggregator : IGrainWithIntegerKey, IAggregator<ArticleState>
    {
        
    }
}