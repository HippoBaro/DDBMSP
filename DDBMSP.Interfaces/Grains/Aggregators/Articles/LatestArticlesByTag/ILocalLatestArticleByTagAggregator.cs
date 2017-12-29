using DDBMSP.Entities.Article;
using DDBMSP.Entities.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag
{
    public interface ILocalLatestArticleByTagAggregator: IGrainWithStringKey, IAggregator<ArticleState>
    {
        
    }
}