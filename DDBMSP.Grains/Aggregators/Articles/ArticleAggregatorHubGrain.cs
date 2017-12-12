using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Aggregators;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles
{
    [StatelessWorker]
    public class ArticleAggregatorHubGrain : Grain, IArticleAggregatorHubGrain
    {
        private Task Broadcast<TTargetGrain>(Immutable<ArticleSummary> article) where TTargetGrain : IGrain
        {
            if (typeof(TTargetGrain) == typeof(ILocalLatestArticleAggregator))
            {
                return GrainFactory.GetGrain<ILocalLatestArticleAggregator>(0).Aggregate(article);
            }

            if (typeof(TTargetGrain) == typeof(ILocalLatestArticleByTagAggregator))
            {
                return Task.WhenAll(article.Value.Tags.Select(tag =>
                    GrainFactory.GetGrain<ILocalLatestArticleByTagAggregator>(tag).Aggregate(article)));
            }

            Console.WriteLine($"Unimplemented aggregator type : {typeof(TTargetGrain).Name}");
            return Task.CompletedTask;
        }

        public Task Aggregate(Immutable<ArticleSummary> article)
        {
            return Task.WhenAll(Broadcast<ILocalLatestArticleAggregator>(article),
                Broadcast<ILocalLatestArticleByTagAggregator>(article));
        }
    }
}