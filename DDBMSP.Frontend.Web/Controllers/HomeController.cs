using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace DDBMSP.Frontend.Web.Controllers
{
    public struct Post
    {
        public string Url { get; set; } //
        public string Title { get; set; } //
        public List<string> Tags { get; set; } //
        public string AuthorId { get; set; } //
        public string AuthorImage { get; set; }
        public string AuthorName { get; set; }
        public string Image { get; set; } //
        public string Excerpt { get; set; } //
        public DateTime CreationDate { get; set; } //
        public string Content { get; set; } //
    }
    
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var friend = GrainClient.GrainFactory.GetGrain<ILatestArticleAggregatorGrain>(0);
            var res = await friend.GetLatestArticles();
            return View("/Views/Index.cshtml", res ?? new List<IArticleData>());
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
            return View("/Views/Tag.cshtml", new Tuple<string, List<IArticleData>>(tag, res ?? new List<IArticleData>()));
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