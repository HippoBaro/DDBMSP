﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.Search;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles
{
    [StatelessWorker]
    public class ArticleAggregatorHubGrain : Grain, IArticleAggregatorHubGrain
    {
        private Task Broadcast<TTargetGrain>(Immutable<ArticleSummary> article) where TTargetGrain : IGrain {
            if (typeof(TTargetGrain) == typeof(ILocalLatestArticleAggregator)) {
                return GrainFactory.GetGrain<ILocalLatestArticleAggregator>(0).Aggregate(article);
            }

            if (typeof(TTargetGrain) == typeof(ILocalSearchArticleAggregator)) {
                return GrainFactory.GetGrain<ILocalSearchArticleAggregator>(0).Aggregate(article);
            }

            if (typeof(TTargetGrain) == typeof(ILocalLatestArticleByTagAggregator)) {
                return Task.WhenAll(article.Value.Tags.Select(tag =>
                    GrainFactory.GetGrain<ILocalLatestArticleByTagAggregator>(tag).Aggregate(article)));
            }

            Console.WriteLine($"Unimplemented aggregator type : {typeof(TTargetGrain).Name}");
            return Task.CompletedTask;
        }

        private Task Broadcast<TTargetGrain>(Immutable<List<ArticleSummary>> articles) where TTargetGrain : IGrain {
            if (typeof(TTargetGrain) == typeof(ILocalLatestArticleAggregator)) {
                return GrainFactory.GetGrain<ILocalLatestArticleAggregator>(0).AggregateRange(articles);
            }

            if (typeof(TTargetGrain) == typeof(ILocalSearchArticleAggregator)) {
                return GrainFactory.GetGrain<ILocalSearchArticleAggregator>(0).AggregateRange(articles);
            }

            if (typeof(TTargetGrain) == typeof(ILocalLatestArticleByTagAggregator)) {
                var dict = new Dictionary<string, List<ArticleSummary>>(articles.Value.Count);

                //That sucks big time
                foreach (var summary in articles.Value) {
                    foreach (var tag in summary.Tags) {
                        if (!dict.ContainsKey(tag))
                            dict.Add(tag, new List<ArticleSummary>());
                        dict[tag].Add(summary);
                    }
                }

                var tasks = new List<Task>(dict.Count);
                tasks.AddRange(dict.Select(tag => GrainFactory.GetGrain<ILocalLatestArticleByTagAggregator>(tag.Key)
                    .AggregateRange(tag.Value.AsImmutable())));
                return Task.WhenAll(tasks);
            }

            Console.WriteLine($"Unimplemented aggregator type : {typeof(TTargetGrain).Name}");
            return Task.CompletedTask;
        }

        public Task Aggregate(Immutable<ArticleSummary> article) {
            return Task.WhenAll(
                Broadcast<ILocalLatestArticleAggregator>(article),
                Broadcast<ILocalLatestArticleByTagAggregator>(article),
                Broadcast<ILocalSearchArticleAggregator>(article));
        }

        public Task AggregateRange(Immutable<List<ArticleSummary>> articles) {
            return Task.WhenAll(
                Broadcast<ILocalLatestArticleAggregator>(articles),
                Broadcast<ILocalLatestArticleByTagAggregator>(articles),
                Broadcast<ILocalSearchArticleAggregator>(articles));
        }
    }
}