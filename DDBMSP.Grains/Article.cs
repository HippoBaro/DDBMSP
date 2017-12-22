using DDBMSP.Interfaces.Grains;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains
{
    [Reentrant]
    public class Article : Grain, IArticle
    {
       
    }
}