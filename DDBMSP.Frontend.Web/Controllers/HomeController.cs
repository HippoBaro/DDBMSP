using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Entities.Search;
using DDBMSP.Entities.User;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.Search;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Frontend.Web.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var friend = GrainClient.GrainFactory.GetGrain<IGlobalLatestArticlesAggregator>(0);
            var res = await friend.GetLatestArticles();
            
            return View("/Views/Index.cshtml", res.Value ?? new List<ArticleSummary>());
        }

        [Route("post/{articleId}")]
        public async Task<IActionResult> Article(Guid articleId)
        {
            try
            {
                var article = await GrainClient.GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0)
                    .Get(articleId);
                
                var friend = GrainClient.GrainFactory.GetGrain<IGlobalLatestArticleByTagAggregator>(0);
                var res = await friend.GetLatestArticlesForTag(article.Value.Tags.First().AsImmutable(), 5);
                
                return View("/Views/Post.cshtml", new Tuple<ArticleState, List<ArticleSummary>>(article.Value, res.Value));
            }
            catch (Exception e)
            {
                return NotFound();
            }
        }

        [Route("author/{authorId}")]
        public async Task<IActionResult> Tag(Guid authorId)
        {
            try
            {
                var user = await GrainClient.GrainFactory.GetGrain<IDistributedHashTable<Guid, UserState>>(0)
                    .Get(authorId);
                return View("/Views/Author.cshtml", user.Value);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Route("tag/{tag}")]
        public async Task<IActionResult> Tag(string tag)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IGlobalLatestArticleByTagAggregator>(0);
            var res = await friend.GetLatestArticlesForTag(tag.AsImmutable(), 100);
            return View("/Views/Tag.cshtml",
                new Tuple<string, List<ArticleSummary>>(tag, res.Value ?? new List<ArticleSummary>()));
        }

        [Route("Error")]
        public IActionResult Error(int? statusCode)
        {
            if (!statusCode.HasValue) return View("/Views/Error.cshtml");
            var viewName = statusCode.ToString();
            return View("/Views/Error.cshtml", viewName);
        }

        [Route("search")]
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
                return NoContent();
            
            var articles = GrainClient.GrainFactory.GetGrain<IGlobalSearchArticleAggregator>(0).GetSearchResult(q.AsImmutable());
            var tags = GrainClient.GrainFactory.GetGrain<IGlobalLatestArticleByTagAggregator>(0).SearchTags(q.AsImmutable());
            
            await Task.WhenAll(articles, tags);

            if (!articles.Result.Value.Any() && tags.Result.Value.Any())
                return NoContent();
            
            return Ok(new SearchResult
            {
                Categories = new Dictionary<string, SearchCategory>
                {
                    {"Articles", new SearchCategory {Name = "Articles", Result = articles.Result.Value}},
                    {"Tags", new SearchCategory {Name = "Tags", Result = tags.Result.Value}}
                }
            });
        }
    }
}