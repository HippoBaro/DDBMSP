using System;
using System.Collections.Generic;
using DDBMSP.Common.Enums;
using DDBMSP.Common.PODs.Core;
using DDBMSP.Common.PODs.User.Components;

namespace DDBMSP.Common.PODs.User
{
    public class UserState : Exist, IIdendityData, IAuthorArticleReferencesData
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string Phone { get; set; }
        public Region Region { get; set; }
        public Language PreferedLanguage { get; set; }
        
        public List<Guid> Articles { get; set; } = new List<Guid>();
    }
}