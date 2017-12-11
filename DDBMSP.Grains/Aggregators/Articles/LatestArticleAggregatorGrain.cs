using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Grains.DataStructures;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;
using Orleans.MultiCluster;

namespace DDBMSP.Grains.Aggregators.Articles
{
    [StatelessWorker]
    public class LatestArticleAggregatorGrain : Grain, ILatestArticleAggregatorGrain
    {
        public CircularFifoStack<ArticleSummary> State { get; set; } = new CircularFifoStack<ArticleSummary>();
        
        public Task Aggregate(Immutable<ArticleSummary> article)
        {
            //State.Push(article);
            return Task.CompletedTask;
        }

        public Task<Immutable<List<ArticleSummary>>> GetLatestArticles(int max = 10)
        {
            return Task.FromResult(State.Any() ? State.Take(max).ToList().AsImmutable() : new Immutable<List<ArticleSummary>>());
        }
    }
}