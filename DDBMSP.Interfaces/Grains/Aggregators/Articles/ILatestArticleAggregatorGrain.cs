using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles
{
    public interface ILatestArticleAggregatorGrain : IGrainWithIntegerKey, IAggregator<IArticleData>
    {
        Task<List<IArticleData>> GetLatestArticles(int max = 10);
    }
}