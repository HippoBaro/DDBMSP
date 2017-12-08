using System;
using System.Collections.Generic;
using DDBMSP.Interfaces.Grains;

namespace DDBMSP.Interfaces.PODs.User.Components
{
    public interface IAuthorArticleReferencesData
    {
        List<IArticle> Articles { get; set; }
    }
}