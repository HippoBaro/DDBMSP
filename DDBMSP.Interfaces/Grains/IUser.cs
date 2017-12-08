using System;
using System.Threading.Tasks;
using DDBMSP.Interfaces.Grains.Core;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.Core;
using DDBMSP.Interfaces.PODs.User;
using DDBMSP.Interfaces.PODs.User.Components;
using Orleans;

namespace DDBMSP.Interfaces.Grains
{
    public interface IUser : IGrainWithGuidKey, IResource<UserState, IUserData>
    {
        Task<Guid> AuthorNewArticle(IArticleData articleData);
    }
}