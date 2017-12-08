using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Aggregators.Articles;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Aggregators.Articles
{
    [StatelessWorker]
    public class ArticleAggregatorHubGrain : Grain, IArticleAggregatorHubGrain
    {
        private Task Broadcast<TTargetGrain>(IArticle article)
            where TTargetGrain : IGrainWithIntegerKey, IArticleListAggregator<IArticle>
        {
            return GrainFactory.GetGrain<TTargetGrain>(0).Aggregate(article);
        }

        public Task Aggregate(IArticle article)
        {
            return Task.WhenAll(Broadcast<ILatestArticleAggregatorGrain>(article),
                Broadcast<ILatestArticleByTagAggregatorGrain>(article));
        }
    }
}