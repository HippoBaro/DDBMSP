using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles
{
    public interface IGlobalLatestArticlesAggregator : IGrainWithIntegerKey, IGlobalAggregator<ArticleSummary>
    {
        Task<Immutable<List<ArticleSummary>>> GetLatestArticles(int max = 10);
    }
}