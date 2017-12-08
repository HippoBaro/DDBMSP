using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles
{
    public interface ILatestArticleByTagAggregatorGrain: IGrainWithIntegerKey, IArticleListAggregator<IArticle>
    {
        Task<List<IArticleData>> GetLatestArticlesForTag(string tag, int max = 10);
    }
}