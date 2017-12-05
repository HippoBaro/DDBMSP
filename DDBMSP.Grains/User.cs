using System;
using System.Threading.Tasks;
using DDBMSP.Interfaces;
using Orleans;

namespace DDBMSP.Grains
{
    public class User : Grain, IUser
    {
        public Task Test()
        {
            return Task.CompletedTask;
        }
    }
}