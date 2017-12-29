using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.Search;
using Lucene.Net.Support;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.Search
{
    [StatelessWorker]
    public class LocalSearchArticleAggregator : Grain<LinkedList<ArticleState>>, ILocalSearchArticleAggregator
    {
        private int _newSinceLastReport;

        public override Task OnActivateAsync() {
            State = new LinkedList<ArticleState>();
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(5000, 8000));
            RegisterTimer(Report, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }

        public Task Aggregate(ArticleState article) {
            State.AddFirst(article);
            ++_newSinceLastReport;
            return Task.CompletedTask;
        }

        public Task AggregateRange(List<ArticleState> articles) {
            State.AddAll(articles);
            _newSinceLastReport += articles.Count;
            return Task.CompletedTask;
        }

        private Task Report(object _) {
            if (!(State.Count > 0)) return Task.CompletedTask;

            var aggregator = GrainFactory.GetGrain<IGlobalSearchArticleAggregator>(0);
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