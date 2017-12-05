using System.Threading.Tasks;
using DDBMSP.Grains.Contracts;
using Orleans;

namespace DDBMSP.Grains
{
    public class User : Orleans.Grain, IUserGrain
    {
        public Task<string> Walk()
        {
            return Task.FromResult(this.GetPrimaryKeyString() + " : I'm walking !!!");
        }
    }
}