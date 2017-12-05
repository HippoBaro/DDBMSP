using System.Threading.Tasks;
using Orleans;

namespace DDBMSP.GrainsContract
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task<string> Walk();
    }
}