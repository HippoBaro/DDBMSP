using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Interfaces.Grains.Aggregators.Articles.LatestArticlesByTag;
using Lucene.Net.Support;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles.LatestArticlesByTag
{
    [StatelessWorker]
    public class LocalLatestArticleByTagAggregator : Grain<OrderedList<ArticleState>>, ILocalLatestArticleByTagAggregator
    {   
        public override Task OnActivateAsync()
        {
            State = new OrderedList<ArticleState>();
            var targetTicks = TimeSpan.FromMilliseconds(RadomProvider.Instance.Next(3000, 10000));
            RegisterTimer(Report, this, targetTicks, targetTicks);
            return base.OnActivateAsync();
        }
        
        public Task Aggregate(ArticleState article)
        {
            State.Add(article);
            return Task.CompletedTask;
        }

        public Task AggregateRange(List<ArticleState> articles) {
            State.AddRange(articles);
            return Task.CompletedTask;
        }

        private Task Report(object _)
        {
            if (!(State.Count > 0)) return Task.CompletedTask;
            
            State.Sort((summary, articleSummary) => DateTime.Compare(articleSummary.CreationDate, summary.CreationDate));
            var aggregator = GrainFactory.GetGrain<IGlobalLatestArticleByTagAggregator>(0);
            var task = aggregator.AggregateRange(this.GetPrimaryKeyString(),
                State.Take(100).Select(state => state.Summarize()).ToList());
            State.Clear();
            return task;
        }
    }
}