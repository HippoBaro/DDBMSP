using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Core.DistributedHashTable
{
    [Reentrant]
    public class DistributedHashTableBucket<TKey, TValue> : SingleWriterMultipleReadersGrain, IDistributedHashTableBucket<TKey, TValue>
    {
        private Dictionary<TKey, TValue> Elements { get; } = new Dictionary<TKey, TValue>(30000);
        
        public Task<Immutable<TValue>> Get(Immutable<TKey> key) => Task.FromResult(Elements[key.Value].AsImmutable());

        public Task Set(Immutable<TKey> key, Immutable<TValue> value) {
            Task Set() {
                if (Elements.ContainsKey(key.Value))
                    Elements[key.Value] = value.Value;
                else
                    Elements.Add(key.Value, value.Value);
                return Task.CompletedTask;
            }

            return SerialExecutor.AddNext(Set);
        }

        public Task SetRange(Immutable<List<KeyValuePair<TKey, TValue>>> keyvalues) {
            Task Set() {
                foreach (var keyval in keyvalues.Value) {
                    Elements.Add(keyval.Key, keyval.Value);
                }
                return Task.CompletedTask;
            }
            return SerialExecutor.AddNext(Set);
        }

        public Task<int> Count() => Task.FromResult(Elements.Count);
    }
}