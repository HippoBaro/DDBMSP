using System.Collections.Generic;
using Newtonsoft.Json;

namespace DDBMSP.Entities.Search
{
    public class SearchCategory
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("results")]
        public List<Dictionary<string, string>> Result { get; set; }
    }
    
    public class SearchResult
    {
        [JsonProperty("results")]
        public Dictionary<string, SearchCategory> Categories { get; set; }
    }
}