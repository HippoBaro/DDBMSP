using DDBMSP.Entities.Article;
using DDBMSP.Entities.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles
{
    public interface ILocalLatestArticleAggregator : IGrainWithIntegerKey, IAggregator<ArticleState>
    {
        
    }
}