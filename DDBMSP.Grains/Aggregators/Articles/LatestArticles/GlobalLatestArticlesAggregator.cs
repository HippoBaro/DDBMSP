using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.LatestArticles
{
    [Reentrant]
    class GlobalLatestArticlesAggregator : SingleWriterMultipleReadersGrain, IGlobalLatestArticlesAggregator
    {
        private List<ArticleSummary> State { get; } = new List<ArticleSummary>();

        public Task Aggregate(Immutable<ArticleSummary> articles) {
            Task Aggregate() {
                var index = State.BinarySearch(articles.Value,
                    Comparer<ArticleSummary>.Create((summary, articleSummary) =>
                        DateTime.Compare(articleSummary.CreationDate, summary.CreationDate)));
                if (index < 0)
                    State.Insert(~index, articles.Value);
                State.RemoveRange(100, int.MaxValue);
                return Task.CompletedTask;
            }

            return SerialExecutor.AddNext(Aggregate);
        }

        public Task AggregateRange(Immutable<List<ArticleSummary>> articles) {
            Task AggregateRange() {
                var index = State.BinarySearch(articles.Value.First(),
                    Comparer<ArticleSummary>.Create((summary, articleSummary) =>
                        DateTime.Compare(articleSummary.CreationDate, summary.CreationDate)));
                if (index < 0)
                    State.InsertRange(~index, articles.Value);
                State.RemoveRange(100, int.MaxValue);
                return Task.CompletedTask;
            }

            return SerialExecutor.AddNext(AggregateRange);
        }

        public Task<Immutable<List<ArticleSummary>>> GetLatestArticles(int max = 10) =>
            Task.FromResult(State.Take(max).ToList().AsImmutable());
    }
}