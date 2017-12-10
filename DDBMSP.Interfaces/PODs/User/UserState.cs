using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.PODs.Core;
using DDBMSP.Interfaces.PODs.User.Components;

namespace DDBMSP.Interfaces.PODs.User
{
    public class UserState : IUserData, IDataOf<IUserData>, IUserSummary, ISummarizableTo<UserSummary>
    {
        public bool Exists { get; set; }
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
        public List<IArticle> Articles { get; set; } = new List<IArticle>();

        public Task<IUserData> Data() => Task.FromResult((IUserData)this);

        public void Populate(IUserData component)
        {
            Name = component.Name;
            Email = component.Email;
            Gender = component.Gender;
            Phone = component.Phone;
            Region = component.Region;
            PreferedLanguage = component.PreferedLanguage;
            Image = component.Image;
            Department = component.Department;
            University = component.University;
            PreferedTags.AddRange(component.PreferedTags);
            ObtainedCredits = component.ObtainedCredits;
            Articles.AddRange(component.Articles);
        }

        public void Populate(IUserSummary component)
        {
            Exists = component.Exists;
            Id = component.Id;
            Name = component.Name;
            Image = component.Image;
        }

        public Task<UserSummary> Summarize()
        {
            var res = new UserSummary();
            res.Populate(this);
            return Task.FromResult(res);
        }
    }
}