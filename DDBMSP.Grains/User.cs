using System.Threading.Tasks;
using DDBMSP.Common.PODs;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Core;
using Orleans;

namespace DDBMSP.Grains
{
    public class User : StatefulGrain<UserIdentity>, IUser
    {        
        public Task<string> Test()
        {
            return Task.FromResult($"Hello from {this.GetPrimaryKey()}, or {RuntimeIdentity}");
        }
    }
}