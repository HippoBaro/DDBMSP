using System;

namespace DDBMSP.Interfaces.PODs.Core
{
    public interface IExist
    {
        bool Exists { get; set; }
        Guid Id { get; set; }
    }
}