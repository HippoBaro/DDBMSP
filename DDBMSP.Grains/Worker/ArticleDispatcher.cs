using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.User;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using DDBMSP.Interfaces.Grains.Workers;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Worker
{
    [StatelessWorker]
    [Reentrant]
    public class ArticleDispatcher : Grain, IArticleDispatcher
    {
        public Task DispatchNewArticlesFromAuthor(Immutable<UserState> author, Immutable<List<ArticleState>> articles) {
            var dict = GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);
            var dictRange = new Dictionary<Guid, ArticleState>(articles.Value.Count);

            foreach (var article in articles.Value) {
                dictRange.Add(article.Id, article);
            }
            
            return Task.WhenAll(
                dict.SetRange(dictRange.AsImmutable()),
                GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0).AggregateRange(author.Value.Articles.AsImmutable()),
                GrainFactory.GetGrain<IDistributedHashTable<Guid, UserState>>(0).Set(author.Value.Id.AsImmutable(), author.Value.AsImmutable()));
        }
    }
}