using System;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Converters;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.User;
using DDBMSP.Interfaces.PODs.User.Components;
using Newtonsoft.Json;
using Orleans;

namespace DDBMSP.Interfaces.Grains
{
    [JsonConverter(typeof(GrainToGuidConverter))]
    public interface IUser : IGrainWithGuidKey, IResource<UserState, IUserData, UserSummary>
    {
        Task<Guid> AuthorNewArticle(IArticleData articleData);
    }
}