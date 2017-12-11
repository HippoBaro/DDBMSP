using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles
{
    public interface ILatestArticleByTagAggregatorGrain: IGrainWithIntegerKey, IAggregator<ArticleSummary>
    {
        Task<Immutable<List<ArticleSummary>>> GetLatestArticlesForTag(Immutable<string> tag, int max = 10);
    }
}