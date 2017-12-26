﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Entities;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.User;
using DDBMSP.Entities.UserActivity;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using DDBMSP.Interfaces.Grains.Workers;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Worker
{
    [StatelessWorker]
    [Reentrant]
    public class ArticleDispatcher : Grain, IArticleDispatcherWorker
    {
        public Task DispatchStorageUnit(Immutable<StorageUnit> unit) {
            var articles = unit.Value.Articles;
            var author = unit.Value.User;
            var activities = unit.Value.Activities;
            
            var dictArticles = GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);
            var dictArticlesRange = new Dictionary<Guid, ArticleState>(articles.Count);
            
            var dictActivities = GrainFactory.GetGrain<IDistributedHashTable<Guid, List<UserActivityState>>>(0);
            var dictActivitiesRange = new Dictionary<Guid, List<UserActivityState>>(activities.Count);

            for (int i = 0; i < articles.Count; i++) {
                dictArticlesRange.Add(articles[i].Id, articles[i]);
                dictActivitiesRange.Add(articles[i].Id, activities[i]);
            }
            
            return Task.WhenAll(
                dictArticles.SetRange(dictArticlesRange.AsImmutable()),
                dictActivities.SetRange(dictActivitiesRange.AsImmutable()),
                GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0).AggregateRange(author.Articles.AsImmutable()),
                GrainFactory.GetGrain<IDistributedHashTable<Guid, UserState>>(0).Set(author.Id.AsImmutable(), author.AsImmutable())
                ).ContinueWith(task => Task.WhenAll(dictArticles.Commit(), dictActivities.Commit()));
        }
        
        public Task DispatchStorageUnits(Immutable<List<StorageUnit>> units) {
            var tasks = new List<Task>(units.Value.Count);
            tasks.AddRange(units.Value.Select(unit => DispatchStorageUnit(unit.AsImmutable())));
            return Task.WhenAll(tasks);
        }
    }
}