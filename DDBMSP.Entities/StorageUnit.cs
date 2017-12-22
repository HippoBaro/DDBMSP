using System.Collections.Generic;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.User;
using DDBMSP.Entities.UserActivity;

namespace DDBMSP.Entities
{
    public class StorageUnit
    {
        public UserState User { get; set; } //The user info
        public List<ArticleState> Articles { get; set; } //The user's articles
        public List<List<UserActivityState>> Activities { get; set; } //The articles's activities
        public int EntityCount { get; set; }
    }
}
