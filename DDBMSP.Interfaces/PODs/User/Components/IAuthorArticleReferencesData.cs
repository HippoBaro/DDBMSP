using System.Collections.Generic;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.PODs.User.Components
{
    public interface IAuthorArticleReferencesData : IComponentOf<IAuthorArticleReferencesData, UserState>
    {
        List<IArticle> Articles { get; set; }
    }
}