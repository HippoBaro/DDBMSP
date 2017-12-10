﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article;
using Microsoft.AspNetCore.Mvc;
using Orleans;

namespace DDBMSP.Frontend.Web.Controllers.APIs
{
    [Route("api/test")]
    public class TestActorSystemController : Controller
    {
        [HttpGet("article/{id}")]
        public async Task<IActionResult> GetArticle(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IArticle>(id);
            var exist = friend.Exits();
            var sum = friend.Summarize();
            
            if (!await exist)
                return NotFound();
            
            return Ok(await sum);
        }
        
        [HttpPut("user/{id}")]
        public async Task<IActionResult> CreateUser(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            
            if (await friend.Exits())
                return BadRequest();
            
            await friend.Create();
            return Created($"/test/user/{id}", id);
        }
        
        [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            var exist = friend.Exits();
            var sum = friend.Summarize();
            
            if (!await exist)
                return NotFound();
            
            return Ok(await sum);
        }
        
        [HttpGet("user/{id}/articles")]
        public async Task<IActionResult> GetUserArticles(Guid id)
        {
            var friend = GrainClient.GrainFactory.GetGrain<IUser>(id);
            var exist = friend.Exits();
            var sum = friend.Summarize();
            
            if (!await exist)
                return NotFound();
            
            return Ok(await sum);
        }
        
        [HttpPut("user/{id}/article")]
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
        
        [HttpPut("user/{id}/article/{tag}")]
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
        
        [HttpGet("latest")]
        public async Task<IActionResult> GetLastestArticle()
        {
            var friend = GrainClient.GrainFactory.GetGrain<ILatestArticleAggregatorGrain>(0);
            return Ok(await friend.GetLatestArticles());
        }
        
        [HttpGet("latest/{tag}")]
        public async Task<IActionResult> GetLastestArticle(string tag)
        {
            var friend = GrainClient.GrainFactory.GetGrain<ILatestArticleByTagAggregatorGrain>(0);
            return Ok(await friend.GetLatestArticlesForTag(tag));
        }
    }
}