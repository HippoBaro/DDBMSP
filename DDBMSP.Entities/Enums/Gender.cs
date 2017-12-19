using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DDBMSP.Entities.Enums
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum Gender
    {
        Male,
        Female,
        Transgender //Why not
    }
}