using System.Threading.Tasks;

namespace DDBMSP.Interfaces
{
    public interface IStateContainer<TPod>
    {
        Task<TPod> GetState();
        Task SetState(TPod state, bool persist = true);
    }
}