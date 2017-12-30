using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag;
using Orleans.Concurrency;
using Orleans.Providers;

namespace DDBMSP.Grains.Aggregators.Articles.LatestArticlesByTag
{
    [Reentrant]
    [StorageProvider(ProviderName = "RedisStore")]
    public class GlobalLatestArticleByTagAggregator : ScheduledPersistedGrain<Dictionary<string, List<ArticleSummary>>>,
        IGlobalLatestArticleByTagAggregator
    {
        public Task Aggregate(string tag, ArticleSummary article) {
            Task Aggregate() {
                if (!State.ContainsKey(tag))
                    State.Add(tag, new List<ArticleSummary>());

                var index = State[tag].BinarySearch(article,
                    Comparer<ArticleSummary>.Create((summary, articleSummary) =>
                        DateTime.Compare(articleSummary.CreationDate, summary.CreationDate)));
                if (index >= 0) return Task.CompletedTask;
                State[tag].Insert(~index, article);
                State[tag].RemoveRange(100, int.MaxValue);
                return Task.CompletedTask;
            }

            CommitChanges();
            return SerialExecutor.AddNext(Aggregate);
        }

        public Task AggregateRange(string tag, List<ArticleSummary> articles) {
            Task AggregateRange() {
                if (!State.ContainsKey(tag))
                    State.Add(tag, new List<ArticleSummary>());

                foreach (var article in articles) {
                    var index = State[tag].BinarySearch(article,
                        Comparer<ArticleSummary>.Create((summary, articleSummary) =>
                            DateTime.Compare(articleSummary.CreationDate, summary.CreationDate)));
                    if (index >= 0) continue;
                    State[tag].Insert(~index, article);
                    State[tag].RemoveRange(100, int.MaxValue);
                }
                return Task.CompletedTask;
            }

            CommitChanges();
            return SerialExecutor.AddNext(AggregateRange);
        }

        public Task<Immutable<List<ArticleSummary>>> GetLatestArticlesForTag(Immutable<string> tag, int max = 10) =>
            Task.FromResult(State.ContainsKey(tag.Value)
                ? State[tag.Value].Take(max).ToList().AsImmutable()
                : new Immutable<List<ArticleSummary>>());

        public Task<Immutable<List<Dictionary<string, string>>>> SearchTags(Immutable<string> keywords) {
            var res = new List<Dictionary<string, string>>(State.Count);
            var keys = keywords.Value.Split(' ');
            res.AddRange(from key in keys
                from tag in State
                where tag.Key.Contains(key)
                select new Dictionary<string, string> {
                    {"title", tag.Key},
                    {"id", "/tag/" + tag.Key}
                });
            return Task.FromResult(res.AsImmutable());
        }
    }
}