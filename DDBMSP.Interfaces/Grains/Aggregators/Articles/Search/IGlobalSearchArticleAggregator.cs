using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles.Search
{
    public interface IGlobalSearchArticleAggregator : IGrainWithIntegerKey, IAggregator<ArticleSummary>
    {
        Task<Immutable<List<Dictionary<string, string>>>> GetSearchResult(Immutable<string> keywords);
    }
}