using System;
using System.Collections.Generic;
using System.Linq;
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

        public UserActivityState() {
            var test = new List<UserActivityState>();

            test.Count(state => state.Type == UserActivityType.Commented);
        }
    }
}