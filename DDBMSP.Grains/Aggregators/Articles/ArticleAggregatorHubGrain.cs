using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.Search;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles
{
    [StatelessWorker]
    [Reentrant]
    public class ArticleAggregatorHubGrain : Grain, IArticleAggregatorHubGrain
    {
        private Task Broadcast<TTargetGrain>(ArticleState article) where TTargetGrain : IGrain {
            if (typeof(TTargetGrain) == typeof(ILocalLatestArticleAggregator)) {
                return GrainFactory.GetGrain<ILocalLatestArticleAggregator>(0).Aggregate(article);
            }

            if (typeof(TTargetGrain) == typeof(ILocalSearchArticleAggregator)) {
                return GrainFactory.GetGrain<ILocalSearchArticleAggregator>(0).Aggregate(article);
            }

            if (typeof(TTargetGrain) == typeof(ILocalLatestArticleByTagAggregator)) {
                return Task.WhenAll(article.Tags.Select(tag =>
                    GrainFactory.GetGrain<ILocalLatestArticleByTagAggregator>(tag).Aggregate(article)));
            }

            Console.WriteLine($"Unimplemented aggregator type : {typeof(TTargetGrain).Name}");
            return Task.CompletedTask;
        }

        private Task Broadcast<TTargetGrain>(List<ArticleState> articles) where TTargetGrain : IGrain {
            if (typeof(TTargetGrain) == typeof(ILocalLatestArticleAggregator)) {
                return GrainFactory.GetGrain<ILocalLatestArticleAggregator>(0).AggregateRange(articles);
            }

            if (typeof(TTargetGrain) == typeof(ILocalSearchArticleAggregator)) {
                return GrainFactory.GetGrain<ILocalSearchArticleAggregator>(0).AggregateRange(articles);
            }

            if (typeof(TTargetGrain) == typeof(ILocalLatestArticleByTagAggregator)) {
                var dict = new Dictionary<string, List<ArticleState>>();

                //That sucks big time
                foreach (var summary in articles) {
                    foreach (var tag in summary.Tags) {
                        if (!dict.ContainsKey(tag))
                            dict.Add(tag, new List<ArticleState>(articles.Count));
                        dict[tag].Add(summary);
                    }
                }

                var tasks = new List<Task>(dict.Count);
                tasks.AddRange(dict
                    .Select(tag => GrainFactory.GetGrain<ILocalLatestArticleByTagAggregator>(tag.Key)
                    .AggregateRange(tag.Value)));
                return Task.WhenAll(tasks);
            }

            Console.WriteLine($"Unimplemented aggregator type : {typeof(TTargetGrain).Name}");
            return Task.CompletedTask;
        }

        public Task Aggregate(ArticleState article) => Task.WhenAll(
            Broadcast<ILocalLatestArticleAggregator>(article),
            Broadcast<ILocalLatestArticleByTagAggregator>(article),
            Broadcast<ILocalSearchArticleAggregator>(article));

        public Task AggregateRange(List<ArticleState> articles) => Task.WhenAll(
            Broadcast<ILocalLatestArticleAggregator>(articles),
            Broadcast<ILocalLatestArticleByTagAggregator>(articles),
            Broadcast<ILocalSearchArticleAggregator>(articles));
    }
}