using System.Threading.Tasks;
using DDBMSP.Grains.Core;
using DDBMSP.Interfaces.Grains;
using DDBMSP.Interfaces.PODs.Article;

namespace DDBMSP.Grains
{
    public class Article : StatefulGrain<ArticleState>, IArticle
    {
    }
}