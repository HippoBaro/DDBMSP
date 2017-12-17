﻿using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Interfaces.PODs.Article;
using DDBMSP.Interfaces.PODs.User;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Interfaces.Grains.Workers
{
    public interface IArticleDispatcher : IGrainWithIntegerKey
    {
        Task DispatchNewArticlesFromAuthor(Immutable<UserState> author, Immutable<List<ArticleState>> articles);
    }
}