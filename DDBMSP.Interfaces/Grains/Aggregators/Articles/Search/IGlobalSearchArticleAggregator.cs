using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles.Search
{
    public interface IGlobalSearchArticleAggregator : IGrainWithIntegerKey, IGlobalAggregator<ArticleSummary>
    {
        Task<Immutable<List<Dictionary<string, string>>>> GetSearchResult(Immutable<string> keywords);
    }
}