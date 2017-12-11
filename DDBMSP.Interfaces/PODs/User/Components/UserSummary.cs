using System;

namespace DDBMSP.Interfaces.PODs.User.Components
{
    public class UserSummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Uri Image { get; set; }

        public UserSummary(UserState userState)
        {
            Id = userState.Id;
            Name = userState.Name;
            Image = userState.Image;
        }
    }
}