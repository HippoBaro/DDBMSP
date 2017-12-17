using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag
{
    public interface IGlobalLatestArticleByTagAggregator : IGrainWithIntegerKey, IAggregator<string, ArticleSummary>
    {
        Task<Immutable<List<ArticleSummary>>> GetLatestArticlesForTag(Immutable<string> tag, int max = 10);
        Task<Immutable<List<Dictionary<string, string>>>> SearchTags(Immutable<string> keywords);
    }
}