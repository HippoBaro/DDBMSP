using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.LatestArticlesByTag
{
    [StatelessWorker]
    public class LocalLatestArticleByTagAggregator : Grain<CircularFifoStack<ArticleState>>, ILocalLatestArticleByTagAggregator
    {
        private int _newSinceLastReport;
        
        public override Task OnActivateAsync()
        {
            State = new CircularFifoStack<ArticleState>();
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(1000, 5000));
            RegisterTimer(Report, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }
        
        public Task Aggregate(ArticleState article)
        {
            State.Push(article);
            ++_newSinceLastReport;
            return Task.CompletedTask;
        }

        public Task AggregateRange(List<ArticleState> articles) {
            State.Push(articles);
            _newSinceLastReport += articles.Count;
            return Task.CompletedTask;
        }

        private async Task Report(object _)
        {
            if (_newSinceLastReport == 0) return;
            var aggregator = GrainFactory.GetGrain<IGlobalLatestArticleByTagAggregator>(0);
            await aggregator.AggregateRange(this.GetPrimaryKeyString().AsImmutable(),
                State.Take(_newSinceLastReport)
                    .Select(state => state.Summarize())
                    .ToList()
                    .AsImmutable());
            _newSinceLastReport = 0;
        }
    }
}