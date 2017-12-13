using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.LatestArticlesByTag
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

        public Task<Immutable<List<Dictionary<string, string>>>> SearchTags(Immutable<string> keywords)
        {
            var res = new List<Dictionary<string, string>>(State.Count);
            var keys = keywords.Value.Split(' ');
            res.AddRange(from key in keys
                from tag in State
                where tag.Key.Contains(key)
                select new Dictionary<string, string>
                {
                    {"title", tag.Key},
                    {"id", "/tag/" + tag.Key}
                });
            return Task.FromResult(res.AsImmutable());
        }
    }
}