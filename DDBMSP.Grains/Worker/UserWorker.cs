using System;
using System.Threading.Tasks;
using DDBMSP.Entities.User;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using DDBMSP.Interfaces.Grains.Workers;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Worker
{
    [Reentrant]
    [StatelessWorker]
    public class UserWorker : Grain, IUserWorker
    {
        private IDistributedHashTable<Guid, UserState> HashTable =>
            GrainFactory.GetGrain<IDistributedHashTable<Guid, UserState>>(0);

        public Task Create(Immutable<UserState> user) => HashTable.Set(user.Value.Id.AsImmutable(), user);
    }
}