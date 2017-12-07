using DDBMSP.Common.Enums;
using DDBMSP.Common.PODs.Core;

namespace DDBMSP.Common.PODs
{
    public class UserIdentity : Exist
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string Phone { get; set; }
        public Region Region { get; set; }
        public Language PreferedLanguage { get; set; }
    }
}