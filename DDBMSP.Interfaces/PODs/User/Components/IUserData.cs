using DDBMSP.Interfaces.Enums;
using DDBMSP.Interfaces.PODs.Core;

namespace DDBMSP.Interfaces.PODs.User.Components
{
    public interface IUserData : IAuthorArticleReferencesData, IComponentOf<IUserData, UserState>
    {
        string Name { get; set; }
        string Email { get; set; }
        Gender Gender { get; set; }
        string Phone { get; set; }
        Region Region { get; set; }
        Language PreferedLanguage { get; set; }
    }
}