using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.Search;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.Search
{
    [StatelessWorker]
    public class LocalSearchArticleAggregator : Grain, ILocalSearchArticleAggregator
    {
        private List<ArticleSummary> State { get; } = new List<ArticleSummary>();
        private int _newSinceLastReport;

        public override Task OnActivateAsync()
        {
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(5000, 20000));
            RegisterTimer(Report, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }

        public Task Aggregate(Immutable<ArticleSummary> article)
        {
            State.Add(article.Value);
            ++_newSinceLastReport;
            return Task.CompletedTask;
        }

        public Task AggregateRange(Immutable<List<ArticleSummary>> articles) {
            State.AddRange(articles.Value);
            _newSinceLastReport += articles.Value.Count;
            return Task.CompletedTask;
        }

        private async Task Report(object _)
        {
            if (_newSinceLastReport == 0) return;
            
            var aggregator = GrainFactory.GetGrain<IGlobalSearchArticleAggregator>(0);
            await aggregator.AggregateRange(State.Take(_newSinceLastReport).ToList().AsImmutable());
            _newSinceLastReport = 0;
            
            State.Clear();
        }
    }
}