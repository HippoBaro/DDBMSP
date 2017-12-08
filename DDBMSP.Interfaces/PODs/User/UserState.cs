using System;
using System.Collections.Generic;
using DDBMSP.Common.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.PODs.Core;
using DDBMSP.Interfaces.PODs.User.Components;

namespace DDBMSP.Interfaces.PODs.User
{
    public class UserState : Exist, IIdendityData, IAuthorArticleReferencesData
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string Phone { get; set; }
        public Region Region { get; set; }
        public Language PreferedLanguage { get; set; }
        
        public List<IArticle> Articles { get; set; } = new List<IArticle>();
    }
}