using System;
using System.Threading.Tasks;

namespace DDBMSP.Interfaces
{
    public interface IUser : Orleans.IGrainWithIntegerKey
    {
        Task Test();
    }
}