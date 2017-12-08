using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Article.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators.Articles
{
    public interface IArticleListAggregator<in TAggregated> where TAggregated : IGrain
    {
        Task Aggregate(TAggregated grain);
    }
}