using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Grains.Aggregators.Articles
{
    public class LatestArticleByTagAggregatorGrain : Grain<Dictionary<string, Stack<IArticleData>>>,
        ILatestArticleByTagAggregatorGrain
    {
        public Task Aggregate(IArticleData article)
        {
            var data = article;
            foreach (var tag in data.Tags)
            {
                if (!State.ContainsKey(tag))
                    State.Add(tag, new Stack<IArticleData>());
                State[tag].Push(data);
            }
            return Task.CompletedTask;
        }

        public Task<List<IArticleData>> GetLatestArticlesForTag(string tag, int max = 10)
        {
            return Task.FromResult(State.ContainsKey(tag) ? State[tag].Take(max).ToList() : null);
        }
    }
}