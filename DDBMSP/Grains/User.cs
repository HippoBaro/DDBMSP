using System.Threading.Tasks;
using DDBMSP.GrainsContract;
using Orleans;

namespace DDBMSP.Grains
{
    public class User : Grain, IUserGrain
    {
        public Task<string> Walk()
        {
            return Task.FromResult(" : I'm walking !!!");
        }
    }
}