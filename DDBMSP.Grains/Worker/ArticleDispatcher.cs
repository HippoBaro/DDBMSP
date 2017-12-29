﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
 using DDBMSP.Common;
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
    public class ArticleDispatcher : Grain<List<StorageUnit>>, IArticleDispatcherWorker
    {
        public override Task OnActivateAsync() {
            State = new List<StorageUnit>(1000);
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(5000, 10000));
            RegisterTimer(Flush, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }

        private Task DispatchStorageUnit(IReadOnlyCollection<StorageUnit> units) {
            var articles = units.SelectMany(unit => unit.Articles).ToList();
            var author = units.Select(unit => unit.User).ToList();
            var activities = units.SelectMany(unit => unit.Activities).ToList();
            
            var dictArticles = GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);
            var dictArticlesRange = new Dictionary<Guid, ArticleState>(articles.Count);
            
            var dictActivities = GrainFactory.GetGrain<IDistributedHashTable<Guid, List<UserActivityState>>>(0);
            var dictActivitiesRange = new Dictionary<Guid, List<UserActivityState>>(activities.Count);
            
            var dictUsers = GrainFactory.GetGrain<IDistributedHashTable<Guid, UserState>>(0);
            var dictUsersRange = new Dictionary<Guid, UserState>(author.Count);

            for (var i = 0; i < articles.Count; i++) {
                dictArticlesRange.Add(articles[i].Id, articles[i]);
                dictActivitiesRange.Add(articles[i].Id, activities[i]);
            }

            foreach (var user in author) {
                dictUsersRange.Add(user.Id, user);
            }
            
            return Task.WhenAll(
                dictArticles.SetRange(dictArticlesRange.AsImmutable()),
                dictActivities.SetRange(dictActivitiesRange.AsImmutable()),
                dictUsers.SetRange(dictUsersRange.AsImmutable()),
                GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0).AggregateRange(articles)
                );
        }
        
        public Task DispatchStorageUnits(Immutable<List<StorageUnit>> units) {
            State.AddRange(units.Value);
            return Task.CompletedTask;
        }
        
        private async Task Flush(object _) {
            if (State.Count == 0) return;
            await DispatchStorageUnit(State);
            State.Clear();
        }
    }
}