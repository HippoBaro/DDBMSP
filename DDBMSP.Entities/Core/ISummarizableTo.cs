namespace DDBMSP.Entities.Core
{
    public interface ISummarizableTo<out TSummary>
    {
        TSummary Summarize();
    }
}