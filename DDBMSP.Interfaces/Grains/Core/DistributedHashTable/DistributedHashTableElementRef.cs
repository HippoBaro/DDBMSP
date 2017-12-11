namespace DDBMSP.Interfaces.Grains.Core.DistributedHashTable
{
    public class DistributedHashTableElementRef<TKey, TValue>
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }
        private int BucketId { get; set; }

        public DistributedHashTableElementRef(TKey key, TValue value, int bucketId)
        {
            Key = key;
            Value = value;
            BucketId = bucketId;
        }
    }
}