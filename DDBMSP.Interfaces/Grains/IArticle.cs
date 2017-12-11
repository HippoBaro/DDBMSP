using DDBMSP.Interfaces.Converters;
using Newtonsoft.Json;
using Orleans;

namespace DDBMSP.Interfaces.Grains
{
    [JsonConverter(typeof(GrainToGuidConverter))]
    public interface IArticle : IGrainWithGuidKey
    {
        
    }
}