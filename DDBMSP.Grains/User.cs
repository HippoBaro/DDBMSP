﻿using System;
using System.Threading.Tasks;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.User;
using DDBMSP.Interfaces.PODs.User.Components;
using Orleans;

namespace DDBMSP.Grains
{
    public class User : ResourceGrain<UserState, IUserData, UserSummary>, IUser
    {
        private IArticleAggregatorHubGrain _articleAggregatorHubGrain;

        private IArticleAggregatorHubGrain ArticleAggregatorHub =>
            _articleAggregatorHubGrain ??
            (_articleAggregatorHubGrain = GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0));

        public Task<Guid> AuthorNewArticle(IArticleData articleData)
        {
            var article = GrainFactory.GetGrain<IArticle>(Guid.NewGuid());

            var articleState = new ArticleState().Populate(articleData);
            articleState.CreationDate = DateTime.UtcNow;
            articleState.Exists = true;
            articleState.Id = article.GetPrimaryKey();
            articleState.Author = new UserSummary().Populate(State);

            var summary = articleState.SummarizeLocal();
            State.Articles.Add(summary);
            
            ArticleAggregatorHub.Aggregate(summary).Ignore();
            WriteStateAsync().Ignore();
            article.SetState(articleState).Ignore();
            
            return Task.FromResult(article.GetPrimaryKey());
        }
    }
}