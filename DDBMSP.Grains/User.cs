using System;
using System.Threading.Tasks;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.User;
using Orleans;
using Orleans.Streams;

namespace DDBMSP.Grains
{
    public class User : StatefulGrain<UserState>, IUser
    {
        public IArticleAggregatorHubGrain ArticleAggregatorHub => GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0);

        public Task<Guid> AuthorNewArticle(IArticleData articleData)
        {
            var article = GrainFactory.GetGrain<IArticle>(Guid.NewGuid());
            articleData.CreationDate = DateTime.UtcNow;
            articleData.AuthorId = this.GetPrimaryKey();
            var state = (ArticleState) articleData;
            state.Exists = true;
            State.Articles.Add(article);
            
            
            return article.SetState(state).ContinueWith(task => ArticleAggregatorHub.Aggregate(article))
                .ContinueWith(task => article.GetPrimaryKey());
        }
    }
}