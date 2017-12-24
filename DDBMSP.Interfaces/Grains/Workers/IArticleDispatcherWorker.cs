using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Workers
{
    public interface IArticleDispatcherWorker : IGrainWithIntegerKey
    {
        Task DispatchStorageUnit(Immutable<StorageUnit> unit);
        Task DispatchStorageUnits(Immutable<List<StorageUnit>> units);
    }
}