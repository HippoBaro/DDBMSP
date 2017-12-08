using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Grains.Aggregators.Articles
{
    public class LatestArticleAggregatorGrain : Grain<Stack<IArticleData>>, ILatestArticleAggregatorGrain
    {   
        public Task Aggregate(IArticle article)
        {
            return article.GetState().ContinueWith(task => State.Push(task.Result));
        }

        public Task<List<IArticleData>> GetLatestArticles(int max = 10)
        {
            //TODO : Find a way to get rid of .ToList() — useless copies
            return Task.FromResult(State.Skip(Math.Max(0, State.Count - max)).ToList());
        }
    }
}