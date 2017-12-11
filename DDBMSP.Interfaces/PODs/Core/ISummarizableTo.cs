namespace DDBMSP.Interfaces.PODs.Core
{
    public interface ISummarizableTo<out TSummary>
    {
        TSummary Summarize();
    }
}