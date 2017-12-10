using System;
using System.Collections.Generic;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.PODs.Article.Components
{
    public interface IArticleData : IExist, IComponentOf<IArticleData, ArticleState>
    {
        DateTime CreationDate { get; set; }
        string Title { get; set; }
        ArticleCategory Catergory { get; set; }
        string Abstract { get; set; }
        List<string> Tags { get; set; }
        IUser Author { get; set; }
        string AuthorImage { get; set; }
        string AuthorName { get; set; }
        Language Language { get; set; }
        string Content { get; set; }
        Uri Image { get; set; }
        Uri Video { get; set; }
        
    }
}