using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.Core;
using Orleans;

namespace DDBMSP.Interfaces.Grains
{
    public interface IArticle : IGrainWithGuidKey, IResource<ArticleState, IArticleData>
    {
        Task PopulateFromAuthorAndData(IUser user, IArticleData data);
    }
}