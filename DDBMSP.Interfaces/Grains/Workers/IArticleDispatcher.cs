using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.User;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Workers
{
    public interface IArticleDispatcher : IGrainWithIntegerKey
    {
        Task DispatchNewArticlesFromAuthor(UserState author, params ArticleState[] articles);
    }
}