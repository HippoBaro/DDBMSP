using System;
using System.Collections.Generic;
using DDBMSP.Entities.Article.Components;
using DDBMSP.Entities.Core;
using DDBMSP.Entities.Enums;
using DDBMSP.Entities.User.Components;

namespace DDBMSP.Entities.User
{
    public class UserState : ISummarizableTo<UserSummary>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string Phone { get; set; }
        public Region Region { get; set; }
        public Language PreferedLanguage { get; set; }
        public Uri Image { get; set; }
        public string Department { get; set; }
        public string University { get; set; }
        public List<string> PreferedTags { get; set; } = new List<string>();
        public int ObtainedCredits { get; set; }
        public List<ArticleSummary> Articles { get; set; } = new List<ArticleSummary>();

        public UserSummary Summarize() => new UserSummary(this);
    }
}