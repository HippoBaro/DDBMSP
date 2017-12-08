using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Interfaces.PODs.Article;
using Orleans;

namespace DDBMSP.Interfaces.Grains
{
    public interface IArticle : IGrainWithGuidKey, IStateContainer<ArticleState>
    {
    }
}