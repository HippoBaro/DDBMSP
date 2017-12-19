using DDBMSP.Entities.Article.Components;
using DDBMSP.Entities.User.Components;

namespace DDBMSP.Entities.UserActivity
{
    public class UserActivity
    {
        public UserSummary User { get; set; }
        public ArticleSummary Summary { get; set; }
        public bool Shared { get; set; }
    }
}