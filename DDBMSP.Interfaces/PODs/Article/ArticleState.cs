using System;
using System.Collections.Generic;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.Core;
using DDBMSP.Interfaces.PODs.User.Components;

namespace DDBMSP.Interfaces.PODs.Article
{
    public class ArticleState : ISummarizableTo<ArticleSummary>
    {
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Title { get; set; }
        public ArticleCategory Catergory { get; set; }
        public string Abstract { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public UserSummary Author { get; set; }
        public Language Language { get; set; }
        public string Content { get; set; }
        public Uri Image { get; set; }
        public Uri Video { get; set; }

        public ArticleSummary Summarize() => new ArticleSummary(this);
    }
}