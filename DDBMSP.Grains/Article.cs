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
    public class Article : ResourceGrain<ArticleState, IArticleData>, IArticle
    {
        private IArticleAggregatorHubGrain ArticleAggregatorHub => GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0);
        
        public Task PopulateFromAuthorAndData(IUser author, IArticleData data)
        {
            data.Author = author;
            data.CreationDate = DateTime.UtcNow;
            State.Exists = true;
            State.Populate(data);
            ArticleAggregatorHub.Aggregate(State).Ignore();
            return Task.CompletedTask;
        }
    }
}