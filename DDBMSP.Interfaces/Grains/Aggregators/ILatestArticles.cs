using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Common.PODs.Article.Components;
using DDBMSP.Common.PODs.User;
using Orleans;

namespace DDBMSP.Interfaces.Grains.Aggregators
{
    public interface ILatestArticles : IGrainWithGuidKey
    {
        Task<List<IArticleData>> GetLatestArticles(int max = 10);
    }
}