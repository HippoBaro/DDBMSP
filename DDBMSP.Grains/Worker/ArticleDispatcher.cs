using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities;
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
        public Task DispatchStorageUnit(Immutable<StorageUnit> unit) {
            var articles = unit.Value.Articles;
            var author = unit.Value.User;
            var activities = unit.Value.Activities;
            
            var dict = GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);
            var dictRange = new Dictionary<Guid, ArticleState>(articles.Count);

            foreach (var article in articles) {
                dictRange.Add(article.Id, article);
            }
            
            return Task.WhenAll(
                dict.SetRangeUnsafe(dictRange.AsImmutable()),
                GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0).AggregateRange(author.Articles.AsImmutable()),
                GrainFactory.GetGrain<IDistributedHashTable<Guid, UserState>>(0).Set(author.Id.AsImmutable(), author.AsImmutable()));
        }
    }
}