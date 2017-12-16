using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using DDBMSP.Interfaces.Grains.Workers;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.User;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Worker
{
    [StatelessWorker]
    public class ArticleDispatcher : Grain, IArticleDispatcher
    {
        public Task DispatchNewArticlesFromAuthor(UserState author, params ArticleState[] articles)
        {
            var dict = GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);
            var authorSummary = author.Summarize();
            var taskArticle = new List<Task>(articles.Length);
            var taskArticleAgg = new List<Task>(articles.Length);
            foreach (var article in articles)
            {
                article.Id = Guid.NewGuid();
                article.Author = authorSummary;
                author.Articles.Add(article.Summarize());
                taskArticle.Add(dict.Set(article.Id, article));
                taskArticleAgg.Add(GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0).Aggregate(article.Summarize().AsImmutable()));
            }
            return Task.WhenAll(Task.WhenAll(taskArticle), Task.WhenAll(taskArticleAgg), GrainFactory.GetGrain<IDistributedHashTable<Guid, UserState>>(0).Set(author.Id, author));
        }
    }
}