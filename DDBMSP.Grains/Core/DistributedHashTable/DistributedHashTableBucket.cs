using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Common.QueryEngine;
using DDBMSP.Entities.Query;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using Orleans.Concurrency;
using Orleans.Providers;

namespace DDBMSP.Grains.Core.DistributedHashTable
{
    [Reentrant]
    [StorageProvider(ProviderName = "RedisStore")]
    public class DistributedHashTableBucket<TKey, TValue> : SingleWriterMultipleReadersGrain<Dictionary<TKey, TValue>>,
        IDistributedHashTableBucket<TKey, TValue>
    {
        public Task<Immutable<TValue>> Get(Immutable<TKey> key) => Task.FromResult(State[key.Value].AsImmutable());

        public Task Set(Immutable<TKey> key, Immutable<TValue> value) {
            Task Set() {
                if (State.ContainsKey(key.Value))
                    State[key.Value] = value.Value;
                else
                    State.Add(key.Value, value.Value);
                CommitChanges();
                return Task.CompletedTask;
            }

            return SerialExecutor.AddNext(Set);
        }

        public Task SetRange(Immutable<IEnumerable<KeyValuePair<TKey, TValue>>> keyvalues) {
            Task Set() {
                foreach (var keyval in keyvalues.Value) {
                    State.Add(keyval.Key, keyval.Value);
                }
                return Task.CompletedTask;
            }

            return SerialExecutor.AddNext(Set);
        }

        public Task<int> Count() => Task.FromResult(State.Count);
        public Task Commit() {
            CommitChanges();
            return Task.CompletedTask;
        }

        public async Task<Immutable<dynamic>> Query(Immutable<QueryDefinition> queryDefinition) {
            var result = await QueryEngine.Execute(ScriptType.QuerySelector, queryDefinition.Value,
                new QueryContext {
                    __Elements = State.Values.Cast<dynamic>()
                });

            return result.AsImmutable();
        }
    }
}