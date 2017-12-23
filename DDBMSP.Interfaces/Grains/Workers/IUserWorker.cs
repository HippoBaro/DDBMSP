using System.Threading.Tasks;
using DDBMSP.Entities.User;
using DDBMSP.Interfaces.Converters;
using Newtonsoft.Json;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Workers
{
    [JsonConverter(typeof(GrainToGuidConverter))]
    public interface IUserWorker : IGrainWithIntegerKey
    {
        Task Create(Immutable<UserState> user);
    }
}