using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities.UserActivity;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using DDBMSP.Interfaces.Grains.Workers;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Worker
{
    [Reentrant]
    [StatelessWorker]
    public class UserActivityWorker : Grain, IUserActivityWorker
    {
        private IDistributedHashTable<Guid, List<UserActivityState>> HashTable =>
            GrainFactory.GetGrain<IDistributedHashTable<Guid, List<UserActivityState>>>(0);

        public Task SetActivitiesForArticle(Immutable<Guid> guid, Immutable<List<UserActivityState>> activities) =>
            HashTable.Set(guid, activities);

        public Task SetActivitiesForArticles(Immutable<Dictionary<Guid, List<UserActivityState>>> activities) =>
            HashTable.SetRange(activities);

        public async Task AddActivitiesToArticle(Immutable<Guid> guid, Immutable<UserActivityState> activity) {
            var list = await HashTable.Get(guid);
            list.Value.Add(activity.Value);
            await HashTable.Set(guid, list);
        }
    }
}