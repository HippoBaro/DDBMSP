using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Core.DistributedHashTable
{
    [Reentrant]
    public class DistributedHashTableBucket<TKey, TValue> : Grain, IDistributedHashTableBucket<TKey, TValue>
    {
        private Dictionary<TKey, TValue> Elements { get; set; } = new Dictionary<TKey, TValue>(30000);
        private AsyncSerialExecutor SerialExecutor { get; set; } = new AsyncSerialExecutor();
        
        public Task<Immutable<TValue>> Get(Immutable<TKey> key)
        {
            return Task.FromResult(Elements[key.Value].AsImmutable());
        }

        public Task Set(Immutable<TKey> key, Immutable<TValue> value)
        {
            Task set()
            {
                if (Elements.ContainsKey(key.Value))
                    Elements[key.Value] = value.Value;
                else
                    Elements.Add(key.Value, value.Value);
                return Task.CompletedTask;
            }

            return SerialExecutor.AddNext(set);
        }

        public Task<int> Usage()
        {
            return Task.FromResult(Elements.Count);
        }

        public Task<int> Count()
        {
            return Task.FromResult(Elements.Count);
        }
    }
}