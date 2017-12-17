using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using DDBMSP.Interfaces.Grains.Workers;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.User;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Worker
{
    [StatelessWorker]
    public class ArticleDispatcher : Grain, IArticleDispatcher
    {
        public Task DispatchNewArticlesFromAuthor(UserState author, params ArticleState[] articles) {
            var dict = GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);
            var authorSummary = author.Summarize();
            var dictRange = new Dictionary<Guid, ArticleState>(articles.Length);
            
            foreach (var article in articles) {
                article.Id = Guid.NewGuid();
                article.Author = authorSummary;
                author.Articles.Add(article.Summarize());
                dictRange.Add(article.Id, article);
            }
            
            return Task.WhenAll(
                dict.SetRange(dictRange.AsImmutable()),
                GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0).AggregateRange(author.Articles.AsImmutable()),
                GrainFactory.GetGrain<IDistributedHashTable<Guid, UserState>>(0).Set(author.Id, author));
        }
    }
}