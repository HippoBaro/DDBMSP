using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.PODs.Article
{
    public class ArticleState : IExist, IArticleData, ISummarizableTo<IArticleData>
    {
        public bool Exists { get; set; }
        public Guid Id { get; set; }
        
        public DateTime CreationDate { get; set; }
        public string Title { get; set; }
        public ArticleCategory Catergory { get; set; }
        public string Abstract { get; set; }
        public List<string> Tags { get; set; }
        public IUser Author { get; set; }
        public Language Language { get; set; }
        public Uri ContentTextUri { get; set; }
        public Uri ContentImageUri { get; set; }
        public Uri ContentVideoUri { get; set; }
        
        public void Populate(IArticleData component)
        {
            CreationDate = component.CreationDate;
            Title = component.Title;
            Catergory = component.Catergory;
            Abstract = component.Abstract;
            Tags = component.Tags;
            Author = component.Author;
            Language = component.Language;
            ContentTextUri = component.ContentTextUri;
            ContentImageUri = component.ContentImageUri;
            ContentVideoUri = component.ContentVideoUri;
        }

        public Task<IArticleData> Summarize() => Task.FromResult((IArticleData)this);
        
    }
}