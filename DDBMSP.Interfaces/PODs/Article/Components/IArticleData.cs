using System;
using System.Collections.Generic;
using DDBMSP.Common.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.PODs.Article.Components
{
    public interface IArticleData : IComponentOf<IArticleData, ArticleState>
    {
        DateTime CreationDate { get; set; }
        string Title { get; set; }
        ArticleCategory Catergory { get; set; }
        string Abstract { get; set; }
        List<string> Tags { get; set; }
        IUser Author { get; set; }
        Language Language { get; set; }
        Uri ContentTextUri { get; set; }
        Uri ContentImageUri { get; set; }
        Uri ContentVideoUri { get; set; }
    }
}