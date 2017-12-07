using System;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Common.PODs.Article.Components;
using DDBMSP.Common.PODs.User;
using Orleans;

namespace DDBMSP.Interfaces.Grains
{
    public interface IUser : IGrainWithGuidKey, IStateContainer<UserState>
    {
        Task<Guid> AuthorNewArticle(IArticleData articleData);
    }
}