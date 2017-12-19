﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.LatestArticlesByTag
{
    [StatelessWorker]
    public class LocalLatestArticleByTagAggregator : Grain, ILocalLatestArticleByTagAggregator
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

        public Task AggregateRange(Immutable<List<ArticleSummary>> articles) {
            State.Push(articles.Value);
            _newSinceLastReport += articles.Value.Count;
            return Task.CompletedTask;
        }

        private async Task Report(object _)
        {
            if (_newSinceLastReport == 0) return;
            
            var aggregator = GrainFactory.GetGrain<IGlobalLatestArticleByTagAggregator>(0);
            await aggregator.AggregateRange(this.GetPrimaryKeyString().AsImmutable(), State.Take(_newSinceLastReport).ToList().AsImmutable());
            _newSinceLastReport = 0;
        }
    }
}