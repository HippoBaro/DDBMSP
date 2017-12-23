using System;
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
    public class ArticleDispatcherWorker : Grain, IArticleDispatcherWorker
    {
        private IArticleWorker ArticleWorkerWorker => GrainFactory.GetGrain<IArticleWorker>(0);
        private IUserWorker UserWorkerWorker => GrainFactory.GetGrain<IUserWorker>(0);
        private IUserActivityWorker ActivitiesWorkerWorker => GrainFactory.GetGrain<IUserActivityWorker>(0);
        
        public Task DispatchStorageUnit(Immutable<StorageUnit> unit) {
            var articles = unit.Value.Articles;
            var author = unit.Value.User;
            var activities = unit.Value.Activities;
            
            var dictActivitiesRange = new Dictionary<Guid, List<UserActivityState>>(activities.Count);
            for (var i = 0; i < articles.Count; i++) {
                dictActivitiesRange.Add(articles[i].Id, activities[i]);
            }
            
            return Task.WhenAll(
                ArticleWorkerWorker.CreateRange(articles.AsImmutable()),
                ActivitiesWorkerWorker.SetActivitiesForArticles(dictActivitiesRange.AsImmutable()),
                GrainFactory.GetGrain<IArticleAggregatorHubGrain>(0).AggregateRange(author.Articles.AsImmutable()),
                UserWorkerWorker.Create(author.AsImmutable())
                );
        }
        
        public Task DispatchStorageUnits(Immutable<List<StorageUnit>> units) {
            var tasks = new List<Task>(units.Value.Count);
            tasks.AddRange(units.Value.Select(unit => DispatchStorageUnit(unit.AsImmutable())));
            return Task.WhenAll(tasks);
        }
    }
}