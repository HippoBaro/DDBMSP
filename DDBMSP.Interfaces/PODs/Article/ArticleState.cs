using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.PODs.Article
{
    public class ArticleState : IArticleData, ISummarizableTo<IArticleData>
    {
        public bool Exists { get; set; }
        public Guid Id { get; set; }
        
        public DateTime CreationDate { get; set; }
        public string Title { get; set; }
        public ArticleCategory Catergory { get; set; }
        public string Abstract { get; set; }
        public List<string> Tags { get; set; }
        public IUser Author { get; set; }
        public string AuthorImage { get; set; }
        public string AuthorName { get; set; }
        public Language Language { get; set; }
        public string Content { get; set; }
        public Uri Image { get; set; }
        public Uri Video { get; set; }
        
        public void Populate(IArticleData component)
        {
            CreationDate = component.CreationDate;
            Title = component.Title;
            Catergory = component.Catergory;
            Abstract = component.Abstract;
            Tags = component.Tags;
            Author = component.Author;
            AuthorImage = component.AuthorImage;
            AuthorName = component.AuthorName;
            Language = component.Language;
            Content = component.Content;
            Image = component.Image;
            Video = component.Video;
        }

        public Task<IArticleData> Summarize() => Task.FromResult((IArticleData)this);
        
    }
}