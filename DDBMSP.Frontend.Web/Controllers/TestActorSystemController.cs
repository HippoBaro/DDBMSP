using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Common.Enums;
using DDBMSP.Common.PODs.Article;
using DDBMSP.Common.PODs.Article.Components;
using DDBMSP.Common.PODs.User;
using DDBMSP.Common.PODs.User.Components;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace DDBMSP.Frontend.Web.Controllers
{
    public class TestActorSystemController : Controller
    {
        [HttpGet("/test/{id}")]
        public Task<UserState> GetInfo(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            return friend.GetState();
        }
        
        [HttpGet("/test/{id}/articles")]
        public async Task<IAuthorArticleReferencesData> GetArticles(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            return await friend.GetState();
        }
        
        [HttpGet("/test/{id}/createarticle")]
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
            var friend = GrainClient.GrainFactory.GetGrain<ILatestArticles>(Guid.ParseExact("00000000000000000000000000000001", "N"));
            return friend.GetLatestArticles();
        }
    }
}