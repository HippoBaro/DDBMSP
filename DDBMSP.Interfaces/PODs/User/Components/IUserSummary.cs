using System;
using System.Collections.Generic;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.PODs.User.Components
{
    public interface IUserSummary : IExist
    {
        string Name { get; set; }
        Uri Image { get; set; }
    }
    
    public class UserSummary : IUserSummary, IComponentOf<UserSummary, IUserData>
    {
        public bool Exists { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Uri Image { get; set; }

        public UserSummary Populate(IUserData component)
        {
            Exists = component.Exists;
            Id = component.Id;
            Name = component.Name;
            Image = component.Image;
            return this;
        }
    }
}