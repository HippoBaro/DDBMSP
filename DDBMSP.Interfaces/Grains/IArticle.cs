using System.Threading.Tasks;
using DDBMSP.Interfaces.Converters;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.Core;
using Newtonsoft.Json;
using Orleans;

namespace DDBMSP.Interfaces.Grains
{
    [JsonConverter(typeof(GrainToGuidConverter))]
    public interface IArticle : IGrainWithGuidKey, IResource<ArticleState, IArticleData>
    {
        Task CreateFromAuthorAndData(IUser user, IArticleData data);
    }
}