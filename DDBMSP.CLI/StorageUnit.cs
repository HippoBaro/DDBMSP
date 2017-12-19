using System.Collections.Generic;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.User;

namespace DDBMSP.CLI
{
    public class ArticleStorageUnit
    {
        public ArticleState Article { get; set; } // The article
        public List<string> Comments { get; set; } // The article's comments
        public List<string> Activities { get; set; }// The article's traffic
    }
    
    public class StorageUnit
    {
        public UserState User { get; set; } //The user info
        public List<ArticleState> Articles { get; set; } //The user's articles
        public List<string> Comments { get; set; } //The user's comments
        public List<string> Activities { get; set; } //The users reading activities
        public int EntityCount { get; set; }
    }
}
