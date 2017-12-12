using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.LatestArticles
{
    class GlobalLatestArticlesAggregator : Grain, IGlobalLatestArticlesAggregator
    {
        private List<ArticleSummary> State { get; } = new List<ArticleSummary>();

        public Task Aggregate(Immutable<List<ArticleSummary>> articles)
        {
            foreach (var article in articles.Value)
            {
                var index = State.BinarySearch(article,
                    Comparer<ArticleSummary>.Create((summary, articleSummary) =>
                        DateTime.Compare(articleSummary.CreationDate, summary.CreationDate)));
                if (index < 0)
                    State.Insert(~index, article);
                return Task.CompletedTask;
            }
            return Task.CompletedTask;
        }

        public Task<Immutable<List<ArticleSummary>>> GetLatestArticles(int max = 10)
        {
            return Task.FromResult(State.Take(max).ToList().AsImmutable());
        }
    }
}