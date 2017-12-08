using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Grains.Aggregators.Articles
{
    public class LatestArticleAggregatorGrain : Grain<Stack<IArticleData>>, ILatestArticleAggregatorGrain
    {
        public Task Aggregate(IArticleData article)
        {
            State.Push(article);
            return Task.CompletedTask;
        }

        public Task<List<IArticleData>> GetLatestArticles(int max = 10)
        {
            return Task.FromResult(State.Any() ? State.Take(max).ToList() : null);
        }
    }
}