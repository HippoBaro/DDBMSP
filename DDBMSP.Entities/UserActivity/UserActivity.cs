using System;
using DDBMSP.Entities.Enums;
using DDBMSP.Entities.User.Components;

namespace DDBMSP.Entities.UserActivity
{
    public class UserActivityState
    {
        public DateTime CreationDate { get; set; }
        public UserSummary User { get; set; }
        public UserActivityType Type { get; set; }
        public string Comment { get; set; }
    }
}