using System;
using Newtonsoft.Json;

namespace DDBMSP.Interfaces.PODs.Core
{
    public interface IExist
    {
        [JsonIgnore]
        bool Exists { get; set; }
        Guid Id { get; set; }
    }
}