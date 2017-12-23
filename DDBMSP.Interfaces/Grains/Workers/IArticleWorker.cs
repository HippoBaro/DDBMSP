using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using DDBMSP.Interfaces.Converters;
using Newtonsoft.Json;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Workers
{
    [JsonConverter(typeof(GrainToGuidConverter))]
    public interface IArticleWorker : IGrainWithIntegerKey
    {
        Task Create(Immutable<ArticleState> article);
        Task CreateRange(Immutable<List<ArticleState>> article);
    }
}