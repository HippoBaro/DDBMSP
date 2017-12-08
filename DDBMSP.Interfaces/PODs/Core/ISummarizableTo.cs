using System.Threading.Tasks;

namespace DDBMSP.Interfaces.PODs.Core
{
    public interface ISummarizableTo<TSummary>
    {
        Task<TSummary> Summarize();
    }
}