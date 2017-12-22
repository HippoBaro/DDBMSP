using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Querier;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Core.DistributedHashTable
{
    public interface IDistributedHashTableBucket<TKey, TValue> : IGrainWithIntegerKey, IDynamicQueryable
    {
        Task<Immutable<TValue>> Get(Immutable<TKey> key);
        Task Set(Immutable<TKey> key, Immutable<TValue> value);
        Task SetRange(Immutable<IEnumerable<KeyValuePair<TKey, TValue>>> keyvalues);
        Task<int> Count();

        Task<Immutable<Dictionary<TKey, TValue>>> Enumerate();
    }
}