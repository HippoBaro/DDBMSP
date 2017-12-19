using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.Search;
using Lucene.Net.Support;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.Search
{
    [StatelessWorker]
    public class LocalSearchArticleAggregator : Grain, ILocalSearchArticleAggregator
    {
        private LinkedList<ArticleSummary> State { get; } = new LinkedList<ArticleSummary>();
        private int _newSinceLastReport;

        public override Task OnActivateAsync() {
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(15000, 35000));
            RegisterTimer(Report, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }

        public Task Aggregate(Immutable<ArticleSummary> article) {
            State.AddFirst(article.Value);
            ++_newSinceLastReport;
            return Task.CompletedTask;
        }

        public Task AggregateRange(Immutable<List<ArticleSummary>> articles) {
            State.AddAll(articles.Value);
            _newSinceLastReport += articles.Value.Count;
            return Task.CompletedTask;
        }

        private async Task Report(object _) {
            if (_newSinceLastReport == 0) return;

            var aggregator = GrainFactory.GetGrain<IGlobalSearchArticleAggregator>(0);
            await aggregator.AggregateRange(State.Take(_newSinceLastReport).ToList().AsImmutable());
            _newSinceLastReport = 0;

            State.Clear();
        }
    }
}