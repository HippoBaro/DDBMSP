using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.User.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Rest;
using Microsoft.Rest.TransientFaultHandling;
using Orleans;

namespace DDBMSP.Frontend.Web.Controllers
{
    public class TestActorSystemController : Controller
    {
        [HttpGet("/test/article/{id}")]
        public async Task<IActionResult> GetArticle(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IArticle>(id);
            if (!await friend.Exits())
                return NotFound();
            
            return Ok(await friend.Summarize());
        }
        
        [HttpPut("/test/user/{id}")]
        public async Task<IActionResult> CreateUser(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            await friend.Create();
            return Created($"/test/user/{id}", id);
        }
        
        [HttpGet("/test/user/{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            if (!await friend.Exits())
                return NotFound();
            
            return Ok(await friend.Summarize());
        }
        
        [HttpGet("/test/urser/{id}/articles")]
        public async Task<IActionResult> GetUserArticles(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            if (!await friend.Exits())
                return NotFound();
            
            return Ok(await friend.Summarize().ContinueWith(task => (IAuthorArticleReferencesData)task.Result));
        }
        
        [HttpPut("/test/user/{id}/article")]
        public async Task<IActionResult> CreateArticle(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            if (!await friend.Exits())
                return NotFound();
            
            var article = new ArticleState
            {
                Abstract = "Abstract!!",
                Language = Language.English,
                Title = "Super duper title"
            };
            var newId = await friend.AuthorNewArticle(article);
            return Created($"/article/{newId}", newId);
        }
        
        [HttpPut("/test/user/{id}/article/{tag}")]
        public async Task<IActionResult> CreateArticle(Guid id, string tag)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            if (!await friend.Exits())
                return NotFound();
            
            var article = new ArticleState
            {
                Abstract = "Abstract!!",
                Language = Language.English,
                Title = "Super duper title",
                Tags = new List<string> { tag }
            };
            var newId = await friend.AuthorNewArticle(article);
            return Created($"/article/{newId}", newId);
        }
        
        [HttpGet("/test/latest")]
        public async Task<IActionResult> GetLastestArticle()
        {
            var friend = GrainClient.GrainFactory.GetGrain<ILatestArticleAggregatorGrain>(0);
            return Ok(await friend.GetLatestArticles());
        }
        
        [HttpGet("/test/latest/{tag}")]
        public async Task<IActionResult> GetLastestArticle(string tag)
        {
            var friend = GrainClient.GrainFactory.GetGrain<ILatestArticleByTagAggregatorGrain>(0);
            return Ok(await friend.GetLatestArticlesForTag(tag));
        }
    }
}