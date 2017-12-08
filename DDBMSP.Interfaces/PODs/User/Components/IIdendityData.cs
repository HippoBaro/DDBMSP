using DDBMSP.Common.Enums;

namespace DDBMSP.Interfaces.PODs.User.Components
{
    public interface IIdendityData
    {
        string Name { get; set; }
        string Email { get; set; }
        Gender Gender { get; set; }
        string Phone { get; set; }
        Region Region { get; set; }
        Language PreferedLanguage { get; set; }
    }
}