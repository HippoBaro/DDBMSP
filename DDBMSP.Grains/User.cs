using System.Threading.Tasks;
using DDBMSP.Common.Enums;
using DDBMSP.Interfaces;
using Orleans;

namespace DDBMSP.Grains
{
    public class User : Grain, IUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Gender Gender { get; set; }
        public string Phone { get; set; }
        public Region Region { get; set; }
        public Language PreferedLanguage { get; set; }
        
        public Task<string> Test()
        {
            return Task.FromResult($"Hello from {this.GetPrimaryKeyLong()}, or {RuntimeIdentity}");
        }
    }
}