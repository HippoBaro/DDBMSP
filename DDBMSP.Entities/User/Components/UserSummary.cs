using System;

namespace DDBMSP.Entities.User.Components
{
    [Serializable]
    public class UserSummary
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Uri Image { get; set; }

        public UserSummary() { }

        public UserSummary(UserState userState)
        {
            Id = userState.Id;
            Name = userState.Name;
            Image = userState.Image;
        }
    }
}