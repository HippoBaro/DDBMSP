﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace DDBMSP.Interfaces.Enums
{
    [JsonConverter(typeof(StringEnumConverter), true)]
    public enum Language
    {
        English,
        Mandarin,
    }
}