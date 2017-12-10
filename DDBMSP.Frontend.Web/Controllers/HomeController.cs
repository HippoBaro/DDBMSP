using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Orleans;

namespace DDBMSP.Frontend.Web.Controllers
{   
    [Route("")]
    [EnableCors("AllowSpecificOrigin")]
    public class HomeController : Controller
    {
        [HttpGet("")]
        [EnableCors("AllowSpecificOrigin")]
        public async Task<IActionResult> Index()
        {
            var friend = GrainClient.GrainFactory.GetGrain<ILatestArticleAggregatorGrain>(0);
            var res = await friend.GetLatestArticles();
            Response.Headers.Add("Access-Control-Allow-Origin", "https://api.github.com"); 
            return View("/Views/Index.cshtml", res ?? new List<ArticleSummary>());
        }
        
        [Route("post/{articleId}")]
        public async Task<IActionResult> Article(Guid articleId)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IArticle>(articleId);
            var exist = friend.Exits();
            var sum = friend.Data();
            
            if (!await exist)
                return NotFound();
            
            return View("/Views/Post.cshtml", await sum);
        }
        
        [Route("author/{authorId}")]
        public async Task<IActionResult> Tag(Guid authorId)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(authorId);
            var exist = friend.Exits();
            var sum = friend.Data();
            
            if (!await exist)
                return NotFound();
            
            return View("/Views/Author.cshtml", await sum);
        }

        [Route("tag/{tag}")]
        public async Task<IActionResult> Tag(string tag)
        {
            var friend = GrainClient.GrainFactory.GetGrain<ILatestArticleByTagAggregatorGrain>(0);
            var res = await friend.GetLatestArticlesForTag(tag);
            return View("/Views/Tag.cshtml", new Tuple<string, List<ArticleSummary>>(tag, res ?? new List<ArticleSummary>()));
        }
        
        [Route("Error")]
        public IActionResult Error(int? statusCode)
        {
            if (!statusCode.HasValue) return View("/Views/Error.cshtml");
            var viewName = statusCode.ToString();
            return View("/Views/Error.cshtml", viewName);
        }
    }
}