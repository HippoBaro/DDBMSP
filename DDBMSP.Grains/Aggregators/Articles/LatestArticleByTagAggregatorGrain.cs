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
    public class LatestArticleByTagAggregatorGrain : Grain<Dictionary<string, CircularFifoStack<IArticleData>>>,
        ILatestArticleByTagAggregatorGrain
    {
        public Task Aggregate(IArticleData article)
        {
            var data = article;
            foreach (var tag in data.Tags)
            {
                if (!State.ContainsKey(tag))
                    State.Add(tag, new CircularFifoStack<IArticleData>());
                State[tag].Push(data);
            }
            return WriteStateAsync();
        }

        public Task<List<IArticleData>> GetLatestArticlesForTag(string tag, int max = 10)
        {
            return Task.FromResult(State.ContainsKey(tag) ? State[tag].Take(max).ToList() : null);
        }
    }
}