using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.LastestArticlesByTagAggregator
{
    public class GlobalLatestArticleByTagAggregator : Grain, IGlobalLatestArticleByTagAggregator
    {
        private Dictionary<string, List<ArticleSummary>> State { get; } =
            new Dictionary<string, List<ArticleSummary>>();

        public Task Aggregate(Immutable<string> tag, Immutable<List<ArticleSummary>> articles)
        {
            if (!State.ContainsKey(tag.Value))
                State.Add(tag.Value, new List<ArticleSummary>());
            
            foreach (var article in articles.Value)
            {
                var index = State[tag.Value].BinarySearch(article,
                    Comparer<ArticleSummary>.Create((summary, articleSummary) =>
                        DateTime.Compare(articleSummary.CreationDate, summary.CreationDate)));
                if (index < 0)
                    State[tag.Value].Insert(~index, article);
            }
            return Task.CompletedTask;
        }

        public Task<Immutable<List<ArticleSummary>>> GetLatestArticlesForTag(Immutable<string> tag, int max = 10)
        {
            return Task.FromResult(State.ContainsKey(tag.Value)
                ? State[tag.Value].Take(max).ToList().AsImmutable()
                : new Immutable<List<ArticleSummary>>());
        }
    }
}