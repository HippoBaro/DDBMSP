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
    public class LatestArticleAggregatorGrain : Grain<CircularFifoQueue<IArticleData>>, ILatestArticleAggregatorGrain
    {
        public Task Aggregate(IArticleData article)
        {
            State.Enqueue(article);
            return Task.CompletedTask;
        }

        public Task<List<IArticleData>> GetLatestArticles(int max = 10)
        {
            return Task.FromResult(State.Any() ? State.Take(max).ToList() : null);
        }
    }
}