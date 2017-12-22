using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Querier;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Core.DistributedHashTable
{
    public interface IDistributedHashTable<TKey, TValue> : IGrainWithIntegerKey, IDynamicQueryable
    {
        Task<Immutable<TValue>> Get(Immutable<TKey> key);
        Task<Immutable<TValue>> Get(TKey key);
        Task Set(Immutable<TKey> key, Immutable<TValue> value);
        Task Set(TKey key, TValue value);
        Task SetRange(Immutable<Dictionary<TKey, TValue>> dict);
        
        Task<List<int>> GetBucketUsage();

        Task<long> Count();

        Task<Immutable<List<IDistributedHashTableBucket<TKey, TValue>>>> GetBuckets();
    }
}