using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.PODs.Core;
using DDBMSP.Interfaces.PODs.User.Components;

namespace DDBMSP.Interfaces.PODs.User
{
    public class UserState : IUserData, ISummarizableTo<IUserData>
    {
        public bool Exists { get; set; }
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string Phone { get; set; }
        public Region Region { get; set; }
        public Language PreferedLanguage { get; set; }
        
        public List<IArticle> Articles { get; set; } = new List<IArticle>();

        public Task<IUserData> Summarize() => Task.FromResult((IUserData)this);
        
        public void Populate(IAuthorArticleReferencesData component)
        {
            Articles.AddRange(component.Articles);
        }

        public void Populate(IUserData component)
        {
            Name = component.Name;
            Email = component.Email;
            Gender = component.Gender;
            Phone = component.Phone;
            Region = component.Region;
            PreferedLanguage = component.PreferedLanguage;
        }
    }
}