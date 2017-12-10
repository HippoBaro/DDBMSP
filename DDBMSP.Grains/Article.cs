using System;
using System.Threading.Tasks;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Grains
{
    public class Article : ResourceGrain<ArticleState, IArticleData, ArticleSummary>, IArticle
    {
        private IArticleAggregatorHubGrain ArticleAggregatorHub => GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0);
        
        public Task CreateFromAuthorAndData(IUser author, IArticleData data)
        {
            data.Author = author;
            data.CreationDate = DateTime.UtcNow;
            State.Populate(data);
            Create().ContinueWith(task => ArticleAggregatorHub.Aggregate(State).Ignore());
            WriteStateAsync().Ignore();
            return Task.CompletedTask;
        }
    }
}