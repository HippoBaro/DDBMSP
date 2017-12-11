using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Grains.DataStructures;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles
{
    [StatelessWorker]
    public class LatestArticleByTagAggregatorGrain : Grain,
        ILatestArticleByTagAggregatorGrain
    {
        private Dictionary<string, CircularFifoStack<ArticleSummary>> State { get; set; } = new Dictionary<string, CircularFifoStack<ArticleSummary>>();
        
        public Task Aggregate(Immutable<ArticleSummary> article)
        {
            var data = article;
            foreach (var tag in data.Value.Tags)
            {
                //if (!State.ContainsKey(tag))
                ///    State.Add(tag, new CircularFifoStack<ArticleSummary>());
                //State[tag].Push(data);
            }
            return Task.CompletedTask;
        }

        public Task<Immutable<List<ArticleSummary>>> GetLatestArticlesForTag(Immutable<string> tag, int max = 10)
        {
            return Task.FromResult(State.ContainsKey(tag.Value) ? State[tag.Value].Take(max).ToList().AsImmutable() : new Immutable<List<ArticleSummary>>());
        }
    }
}