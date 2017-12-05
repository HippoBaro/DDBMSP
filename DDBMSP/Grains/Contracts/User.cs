using System;
using System.Threading.Tasks;
using Orleans;

namespace DDBMSP.Grains.Contracts
{
    public interface IUserGrain : IGrainWithIntegerKey
    {
        Task<string> Walk();
    }
}