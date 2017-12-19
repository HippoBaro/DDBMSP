using System;
using DDBMSP.Entities.User.Components;

namespace DDBMSP.Entities.Comment
{
    public class Comment
    {
        public string Id { get; set; }
        public UserSummary Author { get; set; }
        public string Content { get; set; }
        public DateTime CreationDate { get; set; }
    }
}