using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Core.DistributedHashTable
{
    [StatelessWorker]
    public class DistributedHashTable<TKey, TValue> : Grain, IDistributedHashTable<TKey, TValue>
    {
        private const int BucketsNumber = 100;
        
        public Task<Immutable<TValue>> Get(Immutable<TKey> key)
        {
            // Calculate the hash code of the key, eliminate negative values.
            var hashCode = key.GetHashCode() & 0x7FFFFFFF;
            var targetBucket = hashCode % BucketsNumber;

            return GrainFactory.GetGrain<IDistributedHashTableBucket<TKey, TValue>>(targetBucket).Get(key);
        }

        public Task<Immutable<TValue>> Get(TKey key) => Get(key.AsImmutable());

        public Task Set(Immutable<TKey> key, Immutable<TValue> value)
        {
            // Calculate the hash code of the key, eliminate negative values.
            var hashCode = key.GetHashCode() & 0x7FFFFFFF;
            var targetBucket = hashCode % BucketsNumber;
            
            return GrainFactory.GetGrain<IDistributedHashTableBucket<TKey, TValue>>(targetBucket).Set(key, value);
        }

        public Task Set(TKey key, TValue value) => Set(key.AsImmutable(), value.AsImmutable());
        
        public async Task<List<int>> GetBucketUsage()
        {
            var ret = new List<int>(BucketsNumber);
            for (var i = 0; i < BucketsNumber; i++)
            {
                ret.Add(await GrainFactory.GetGrain<IDistributedHashTableBucket<TKey, TValue>>(i).Usage());
            }
            return ret;
        }

        public async Task<long> Count()
        {
            var tasks = new List<Task<int>>(BucketsNumber);
            for (var i = 0; i < BucketsNumber; i++)
            {
                tasks.Add(GrainFactory.GetGrain<IDistributedHashTableBucket<TKey, TValue>>(i).Count());
            }
            return (await Task.WhenAll(tasks)).Sum();
        }
    }
}