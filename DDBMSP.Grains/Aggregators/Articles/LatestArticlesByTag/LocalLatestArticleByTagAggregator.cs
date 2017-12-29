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
    public class LocalLatestArticleByTagAggregator : Grain<List<ArticleState>>, ILocalLatestArticleByTagAggregator
    {   
        public override Task OnActivateAsync()
        {
            State = new List<ArticleState>();
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(1000, 5000));
            RegisterTimer(Report, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }
        
        public Task Aggregate(ArticleState article)
        {
            var index = State.BinarySearch(article,
                Comparer<ArticleState>.Create((summary, articleSummary) =>
                    DateTime.Compare(articleSummary.CreationDate, summary.CreationDate)));
            if (index >= 0) return Task.CompletedTask;
            State.Insert(~index, article);
            State.RemoveRange(100, int.MaxValue);
            return Task.CompletedTask;
        }

        public Task AggregateRange(List<ArticleState> articles) {
            foreach (var article in articles) {
                var index = State.BinarySearch(article,
                    Comparer<ArticleState>.Create((summary, articleSummary) =>
                        DateTime.Compare(articleSummary.CreationDate, summary.CreationDate)));
                if (index >= 0) continue;
                State.Insert(~index, article);
                State.RemoveRange(100, int.MaxValue);
            }
            return Task.CompletedTask;
        }

        private Task Report(object _)
        {
            if (!(State.Count > 0)) return Task.CompletedTask;
            
            var aggregator = GrainFactory.GetGrain<IGlobalLatestArticleByTagAggregator>(0);
            var task = aggregator.AggregateRange(this.GetPrimaryKeyString(),
                State.Take(100).Select(state => state.Summarize()).ToList());
            State.Clear();
            return task;
        }
    }
}