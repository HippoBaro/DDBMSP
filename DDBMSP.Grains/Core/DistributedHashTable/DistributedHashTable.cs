using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common.QueryEngine;
using DDBMSP.Entities.Query;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Core.DistributedHashTable
{
    [StatelessWorker]
    [Reentrant]
    public class DistributedHashTable<TKey, TValue> : Grain, IDistributedHashTable<TKey, TValue>
    {
        private const int BucketsNumber = 24;

        public Task<Immutable<TValue>> Get(Immutable<TKey> key) {
            // Calculate the hash code of the key, eliminate negative values.
            var hashCode = key.Value.GetHashCode() & 0x7FFFFFFF;
            var targetBucket = hashCode % BucketsNumber;

            return GrainFactory.GetGrain<IDistributedHashTableBucket<TKey, TValue>>(targetBucket).Get(key);
        }

        public Task<Immutable<TValue>> Get(TKey key) => Get(key.AsImmutable());

        public Task Set(Immutable<TKey> key, Immutable<TValue> value) {
            // Calculate the hash code of the key, eliminate negative values.
            var hashCode = key.Value.GetHashCode() & 0x7FFFFFFF;
            var targetBucket = hashCode % BucketsNumber;

            return GrainFactory.GetGrain<IDistributedHashTableBucket<TKey, TValue>>(targetBucket).Set(key, value);
        }

        public Task Set(TKey key, TValue value) => Set(key.AsImmutable(), value.AsImmutable());

        public Task SetRange(Immutable<Dictionary<TKey, TValue>> dict) {
            var dispatch = new Dictionary<int, List<KeyValuePair<TKey, TValue>>>(dict.Value.Count);

            foreach (var value in dict.Value) {
                // Calculate the hash code of the key, eliminate negative values.
                var hashCode = value.Key.GetHashCode() & 0x7FFFFFFF;
                var targetBucket = hashCode % BucketsNumber;
                if (!dispatch.ContainsKey(targetBucket))
                    dispatch.Add(targetBucket, new List<KeyValuePair<TKey, TValue>>());
                dispatch[targetBucket].Add(value);
            }

            var tasks = new List<Task>(dispatch.Count);
            tasks.AddRange(dispatch.Select(d =>
                GrainFactory.GetGrain<IDistributedHashTableBucket<TKey, TValue>>(d.Key)
                    .SetRange(d.Value.AsEnumerable().AsImmutable())));
            return Task.WhenAll(tasks);
        }

        public async Task<List<int>> GetBucketUsage() {
            var ret = new List<int>(BucketsNumber);
            for (var i = 0; i < BucketsNumber; i++) {
                ret.Add(await GrainFactory.GetGrain<IDistributedHashTableBucket<TKey, TValue>>(i).Count());
            }
            return ret;
        }

        public async Task<long> Count() {
            var tasks = new List<Task<int>>(BucketsNumber);
            for (var i = 0; i < BucketsNumber; i++) {
                tasks.Add(GrainFactory.GetGrain<IDistributedHashTableBucket<TKey, TValue>>(i).Count());
            }
            return (await Task.WhenAll(tasks)).Sum();
        }

        public async Task<Immutable<dynamic>> Query(Immutable<QueryDefinition> queryDefinition) {
            var tasks = new List<Task<Immutable<dynamic>>>(BucketsNumber);
            for (var i = 0; i < BucketsNumber; i++) {
                tasks.Add(GrainFactory.GetGrain<IDistributedHashTableBucket<TKey, TValue>>(i).Query(queryDefinition));
            }
            
            await Task.WhenAll(tasks);
            try {
                return new Immutable<dynamic>(await QueryEngine.Execute(ScriptType.QueryAggregator,
                    queryDefinition.Value,
                    new QueryContext { __TaskResult = tasks.Select(task => task.Result.Value) }));
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }
    }
}