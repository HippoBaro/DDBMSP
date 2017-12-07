using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common.PODs.Article.Components;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators;
using Orleans;

namespace DDBMSP.Grains.Aggregators
{
    [ImplicitStreamSubscription("Articles")]
    public class LatestArticles : AggregatorGrain<IArticle>, ILatestArticles
    {
        public static readonly Guid Id = Guid.ParseExact("00000000000000000000000000000001", "N");
        
        private List<IArticleData> Articles { get; } = new List<IArticleData>();
        
        public override Task OnActivateAsync()
        {
            return base.OnActivateAsync("Articles");
        }

        protected override Task Aggregate(IArticle newValue)
        {
            return newValue.GetState().ContinueWith(task => Articles.Add(task.Result));
        }

        public Task<List<IArticleData>> GetLatestArticles(int max = 10)
        {
            return Task.FromResult(Articles.Skip(Math.Max(0, Articles.Count - max)).ToList());
        }
    }
}