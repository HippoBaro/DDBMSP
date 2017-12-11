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
        public async Task DispatchNewArticlesFromAuthor(UserState author, params ArticleState[] articles)
        {
            var dict = GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);
            var authorSummary = author.Summarize();
            var taskArticle = new List<Task>(articles.Length);
            foreach (var article in articles)
            {
                article.CreationDate = DateTime.UtcNow;
                article.Id = Guid.NewGuid();
                article.Author = authorSummary;
                author.Articles.Add(article.Summarize());
                taskArticle.Add(dict.Set(article.Id, article));
                await GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0).Aggregate(article.Summarize());
            }
            await Task.WhenAll(taskArticle);
            await GrainFactory.GetGrain<IDistributedHashTable<Guid, UserState>>(0).Set(author.Id, author);
        }
    }
}