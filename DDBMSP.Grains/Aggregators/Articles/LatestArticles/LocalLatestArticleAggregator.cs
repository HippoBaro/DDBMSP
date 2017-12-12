using System;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Grains.Core;
using DDBMSP.Grains.DataStructures;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.LatestArticles
{
    [StatelessWorker]
    public class LocalLatestArticleAggregator : Grain, ILocalLatestArticleAggregator
    {
        private CircularFifoStack<ArticleSummary> State { get; } = new CircularFifoStack<ArticleSummary>();
        private int _newSinceLastReport;

        public override Task OnActivateAsync()
        {
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(1000, 5000));
            RegisterTimer(Report, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }

        public Task Aggregate(Immutable<ArticleSummary> article)
        {
            State.Push(article.Value);
            ++_newSinceLastReport;
            return Task.CompletedTask;
        }

        private async Task Report(object _)
        {
            if (_newSinceLastReport == 0) return;
            
            var aggregator = GrainFactory.GetGrain<IGlobalLatestArticlesAggregator>(0);
            await aggregator.Aggregate(State.Take(_newSinceLastReport).ToList().AsImmutable());
            _newSinceLastReport = 0;
        }
    }
}