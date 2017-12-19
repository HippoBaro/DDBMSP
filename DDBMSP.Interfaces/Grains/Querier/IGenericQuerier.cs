using System.Threading.Tasks;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Querier
{
    public interface IGenericQuerier : IGrainWithIntegerKey
    {
        Task<dynamic> Execute();
    }
}