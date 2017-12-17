using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Orleans;

namespace DDBMSP.Interfaces.Converters
{
    public class GrainToGuidConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var grain = value as IGrainWithGuidKey;
            var t = JToken.FromObject(grain.GetPrimaryKey());
            t.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) => throw new NotImplementedException();

        public override bool CanConvert(Type objectType) => true;
    }
}