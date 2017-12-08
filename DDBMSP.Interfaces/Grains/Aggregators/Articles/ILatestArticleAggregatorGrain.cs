using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles
{
    public interface ILatestArticleAggregatorGrain : IGrainWithIntegerKey, IArticleListAggregator<IArticle>
    {
        Task<List<IArticleData>> GetLatestArticles(int max = 10);
    }
}