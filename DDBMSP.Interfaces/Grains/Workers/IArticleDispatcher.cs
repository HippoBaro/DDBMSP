using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.User;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Workers
{
    public interface IArticleDispatcher : IGrainWithIntegerKey
    {
        Task DispatchStorageUnit(Immutable<StorageUnit> unit);
    }
}