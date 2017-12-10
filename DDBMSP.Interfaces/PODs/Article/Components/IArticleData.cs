using System;
using System.Collections.Generic;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.PODs.Core;
using DDBMSP.Interfaces.PODs.User.Components;

namespace DDBMSP.Interfaces.PODs.Article.Components
{
    public interface IArticleData : IExist
    {
        DateTime CreationDate { get; set; }
        string Title { get; set; }
        ArticleCategory Catergory { get; set; }
        string Abstract { get; set; }
        List<string> Tags { get; set; }
        UserSummary Author { get; set; }
        Language Language { get; set; }
        string Content { get; set; }
        Uri Image { get; set; }
        Uri Video { get; set; }
        
    }
}