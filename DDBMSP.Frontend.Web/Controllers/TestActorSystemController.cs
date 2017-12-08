using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Common.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.User;
using DDBMSP.Interfaces.PODs.User.Components;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace DDBMSP.Frontend.Web.Controllers
{
    public class TestActorSystemController : Controller
    {
        [HttpGet("/test/article/{id}")]
        public Task<ArticleState> GetArticle(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IArticle>(id);
            return friend.GetState();
        }
        
        [HttpGet("/test/user/{id}")]
        public Task<UserState> GetUser(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            return friend.GetState();
        }
        
        [HttpGet("/test/urser/{id}/articles")]
        public async Task<IAuthorArticleReferencesData> GetUserArticles(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            return await friend.GetState();
        }
        
        [HttpGet("/test/user/{id}/createarticle")]
        public Task<Guid> NewArticle(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            var article = new ArticleState
            {
                Abstract = "Abstract!!",
                Language = Language.English,
                Title = "Super duper title"
            };
            return friend.AuthorNewArticle(article);
        }
        
        [HttpGet("/test/latest")]
        public Task<List<IArticleData>> GetLastestArticle()
        {
            var friend = GrainClient.GrainFactory.GetGrain<ILatestArticleAggregatorGrain>(0);
            return friend.GetLatestArticles();
        }
        
        [HttpGet("/test/latest/{tag}")]
        public Task<List<IArticleData>> GetLastestArticle(string tag)
        {
            var friend = GrainClient.GrainFactory.GetGrain<ILatestArticleByTagAggregatorGrain>(0);
            return friend.GetLatestArticlesForTag(tag);
        }
    }
}