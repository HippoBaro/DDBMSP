using System.Threading.Tasks;
using DDBMSP.Interfaces.Converters;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.Article.Components;
using Newtonsoft.Json;
using Orleans;

namespace DDBMSP.Interfaces.Grains
{
    [JsonConverter(typeof(GrainToGuidConverter))]
    public interface IArticle : IGrainWithGuidKey, IResource<ArticleState, IArticleData, ArticleSummary>
    {
        
    }
}