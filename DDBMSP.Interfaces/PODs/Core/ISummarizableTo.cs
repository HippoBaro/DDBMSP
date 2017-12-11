using System.Threading.Tasks;

namespace DDBMSP.Interfaces.PODs.Core
{
    public interface ISummarizableTo<TSummary>
    {
        Task<TSummary> Summarize();
    }
    
    public interface IDataOf<TData>
    {
        Task<TData> Data();
        Task Populate(TData component, bool persist = true);
    }
}