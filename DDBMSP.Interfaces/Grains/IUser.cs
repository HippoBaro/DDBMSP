using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Common.PODs;
using Orleans;

namespace DDBMSP.Interfaces.Grains
{
    public interface IUser : IGrainWithGuidKey, IStateContainer<UserIdentity>
    {
        Task<string> Test();
    }
}