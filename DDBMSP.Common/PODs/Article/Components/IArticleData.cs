using System;
using System.Collections.Generic;
using DDBMSP.Common.Enums;

namespace DDBMSP.Common.PODs.Article.Components
{
    public interface IArticleData
    {
        DateTime CreationDate { get; set; }
        string Title { get; set; }
        ArticleCategory Catergory { get; set; }
        string Abstract { get; set; }
        List<string> Tags { get; set; }
        Guid AuthorId { get; set; }
        Language Language { get; set; }
        Uri ContentTextUri { get; set; }
        Uri ContentImageUri { get; set; }
        Uri ContentVideoUri { get; set; }
    }
}