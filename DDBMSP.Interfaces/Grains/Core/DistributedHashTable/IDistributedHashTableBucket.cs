using System.Threading.Tasks;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Core.DistributedHashTable
{
    public interface IDistributedHashTableBucket<TKey, TValue> : IGrainWithIntegerKey
    {
        Task<Immutable<TValue>> Get(Immutable<TKey> key);
        Task Set(Immutable<TKey> key, Immutable<TValue> value);
        Task<int> Usage();
        Task<int> Count();
    }
}