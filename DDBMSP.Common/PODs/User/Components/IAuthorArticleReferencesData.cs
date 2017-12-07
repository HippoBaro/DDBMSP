using System;
using System.Collections.Generic;

namespace DDBMSP.Common.PODs.User.Components
{
    public interface IAuthorArticleReferencesData
    {
        List<Guid> Articles { get; set; }
    }
}