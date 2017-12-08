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
    public class LatestArticleByTagAggregatorGrain : Grain<Dictionary<string, Stack<IArticleData>>>,
        ILatestArticleByTagAggregatorGrain
    {
        public async Task Aggregate(IArticle article)
        {
            var data = await article.GetState();
            foreach (var tag in data.Tags)
            {
                if (State[tag] == null)
                    State[tag] = new Stack<IArticleData>();
                State[tag].Push(data);
            }
        }

        public Task<List<IArticleData>> GetLatestArticlesForTag(string tag, int max = 10)
        {
            //TODO : Find a way to get rid of .ToList() — useless copies
            return State.ContainsKey(tag) ? Task.FromResult(State[tag].Skip(Math.Max(0, State[tag].Count - max)).ToList()) : Task.FromResult((List<IArticleData>)null);
        }
    }
}