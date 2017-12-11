using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.Core;
using DDBMSP.Interfaces.PODs.User.Components;

namespace DDBMSP.Interfaces.PODs.Article
{
    public class ArticleState : IArticleData, IDataOf<IArticleData>, IArticleSummary, ISummarizableTo<ArticleSummary>, IComposedBy<ArticleState, IArticleData>
    {
        public bool Exists { get; set; }
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

        public Task<IArticleData> Data() => Task.FromResult(DataLocal());

        Task IDataOf<IArticleData>.Populate(IArticleData component, bool persist)
        {
            CreationDate = component.CreationDate;
            Title = component.Title;
            Catergory = component.Catergory;
            Abstract = component.Abstract;
            Tags = component.Tags;
            Author = component.Author;
            Language = component.Language;
            Content = component.Content;
            Image = component.Image;
            Video = component.Video;
            return Task.CompletedTask;
        }

        public IArticleData DataLocal() => this;

        public Task<ArticleSummary> Summarize() => Task.FromResult(SummarizeLocal());
        public ArticleSummary SummarizeLocal() => new ArticleSummary().Populate(this);

        public ArticleState Populate(IArticleData component)
        {
            CreationDate = component.CreationDate;
            Title = component.Title;
            Catergory = component.Catergory;
            Abstract = component.Abstract;
            Tags = component.Tags;
            Author = component.Author;
            Language = component.Language;
            Content = component.Content;
            Image = component.Image;
            Video = component.Video;
            return this;
        }
    }
}