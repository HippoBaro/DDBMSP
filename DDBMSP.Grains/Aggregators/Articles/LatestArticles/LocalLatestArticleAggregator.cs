﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticles;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.LatestArticles
{
    [StatelessWorker]
    public class LocalLatestArticleAggregator : Grain<OrderedList<ArticleState>>, ILocalLatestArticleAggregator
    {
        private int _newSinceLastReport;

        public override Task OnActivateAsync() {
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(3000, 5000));
            RegisterTimer(Report, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }

        public Task Aggregate(ArticleState article) {
            State.Add(article);
            ++_newSinceLastReport;
            return Task.CompletedTask;
        }

        public Task AggregateRange(List<ArticleState> articles) {
            State.AddRange(articles);
            _newSinceLastReport += articles.Count;
            return Task.CompletedTask;
        }

        private Task Report(object _) {
            if (!(State.Count > 0)) return Task.CompletedTask;
            
            State.Sort((summary, articleSummary) => DateTime.Compare(articleSummary.CreationDate, summary.CreationDate));
            var aggregator = GrainFactory.GetGrain<IGlobalLatestArticlesAggregator>(0);
            var task = aggregator.AggregateRange(
                State.Take(_newSinceLastReport)
                    .Select(state => state.Summarize())
                    .ToList());
            _newSinceLastReport = 0;
            State.Clear();
            return task;
        }
    }
}