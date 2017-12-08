using System;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Interfaces.PODs.Article.Components;
using DDBMSP.Interfaces.PODs.User;
using Orleans;

namespace DDBMSP.Interfaces.Grains
{
    public interface IUser : IGrainWithGuidKey, IStateContainer<UserState>
    {
        Task<Guid> AuthorNewArticle(IArticleData articleData);
    }
}