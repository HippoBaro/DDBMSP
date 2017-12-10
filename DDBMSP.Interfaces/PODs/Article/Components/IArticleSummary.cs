using System;
using System.Collections.Generic;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.PODs.Core;
using DDBMSP.Interfaces.PODs.User.Components;

namespace DDBMSP.Interfaces.PODs.Article.Components
{
    public interface IArticleSummary : IExist
    {
        DateTime CreationDate { get; set; }
        string Title { get; set; }
        string Abstract { get; set; }
        List<string> Tags { get; set; }
        UserSummary Author { get; set; }
        Uri Image { get; set; }
    }
    
    public class ArticleSummary : IArticleSummary, IComponentOf<ArticleSummary, IArticleData>
    {
        public bool Exists { get; set; }
        public Guid Id { get; set; }
        public DateTime CreationDate { get; set; }
        public string Title { get; set; }
        public string Abstract { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public UserSummary Author { get; set; }
        public Uri Image { get; set; }

        public ArticleSummary Populate(IArticleData component)
        {
            Exists = component.Exists;
            Id = component.Id;
            CreationDate = component.CreationDate;
            Title = component.Title;
            Abstract = component.Abstract;
            Tags = component.Tags;
            Author = component.Author;
            Image = component.Image;
            return this;
        }
    }
}