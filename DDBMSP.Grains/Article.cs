using System.Threading.Tasks;
using DDBMSP.Common.PODs.Article;
using DDBMSP.Common.PODs.User;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains;

namespace DDBMSP.Grains
{
    public class Article : StatefulGrain<ArticleState>, IArticle
    {
    }
}