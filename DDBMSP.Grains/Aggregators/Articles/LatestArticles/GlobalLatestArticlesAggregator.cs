using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles;
using Orleans.Concurrency;
using Orleans.Providers;

namespace DDBMSP.Grains.Aggregators.Articles.LatestArticles
{
    [Reentrant]
    [StorageProvider(ProviderName = "RedisStore")]
    class GlobalLatestArticlesAggregator : ScheduledPersistedGrain<List<ArticleSummary>>, IGlobalLatestArticlesAggregator
    {
        public Task Aggregate(ArticleSummary articles) {
            Task Aggregate() {
                var index = State.BinarySearch(articles,
                    Comparer<ArticleSummary>.Create((summary, articleSummary) =>
                        DateTime.Compare(articleSummary.CreationDate, summary.CreationDate)));
                if (index < 0)
                    State.Insert(~index, articles);
                State.RemoveRange(100, int.MaxValue);
                return Task.CompletedTask;
            }

            CommitChanges();
            return SerialExecutor.AddNext(Aggregate);
        }

        public Task AggregateRange(List<ArticleSummary> articles) {
            Task AggregateRange() {
                var index = State.BinarySearch(articles.First(),
                    Comparer<ArticleSummary>.Create((summary, articleSummary) =>
                        DateTime.Compare(articleSummary.CreationDate, summary.CreationDate)));
                if (index < 0)
                    State.InsertRange(~index, articles);
                State.RemoveRange(100, int.MaxValue);
                return Task.CompletedTask;
            }

            CommitChanges();
            return SerialExecutor.AddNext(AggregateRange);
        }

        public Task<Immutable<List<ArticleSummary>>> GetLatestArticles(int max = 10) =>
            Task.FromResult(State.Take(max).ToList().AsImmutable());
    }
}