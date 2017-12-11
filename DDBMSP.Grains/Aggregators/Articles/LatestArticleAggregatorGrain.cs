using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Grains.DataStructures;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.MultiCluster;

namespace DDBMSP.Grains.Aggregators.Articles
{
    [OneInstancePerCluster]
    public class LatestArticleAggregatorGrain : Grain<CircularFifoStack<ArticleSummary>>, ILatestArticleAggregatorGrain
    {
        public Task Aggregate(ArticleSummary article)
        {
            State.Push(article);
            return WriteStateAsync();
        }

        public Task<List<ArticleSummary>> GetLatestArticles(int max = 10)
        {
            return Task.FromResult(State.Any() ? State.Take(max).ToList() : null);
        }
    }
}