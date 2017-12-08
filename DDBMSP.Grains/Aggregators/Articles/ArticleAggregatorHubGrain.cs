using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Aggregators;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles
{
    [StatelessWorker]
    public class ArticleAggregatorHubGrain : Grain, IArticleAggregatorHubGrain
    {
        private Task Broadcast<TTargetGrain>(IArticleData article)
            where TTargetGrain : IGrainWithIntegerKey, IAggregator<IArticleData>
        {
            return GrainFactory.GetGrain<TTargetGrain>(0).Aggregate(article);
        }

        public Task Aggregate(IArticleData article)
        {
            return Task.WhenAll(Broadcast<ILatestArticleAggregatorGrain>(article),
                Broadcast<ILatestArticleByTagAggregatorGrain>(article));
        }
    }
}