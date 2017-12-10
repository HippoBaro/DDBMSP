using System;
using System.Collections.Generic;
using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.PODs.User.Components
{
    public interface IUserData : IExist, IComponentOf<IUserData, UserState>
    {
        string Name { get; set; }
        string Email { get; set; }
        Gender Gender { get; set; }
        string Phone { get; set; }
        Region Region { get; set; }
        Language PreferedLanguage { get; set; }
        Uri Image { get; set; }
        string Department { get; set; }
        string University { get; set; }
        List<string> PreferedTags { get; set; }
        int ObtainedCredits { get; set; }
        List<IArticle> Articles { get; set; }
    }
}