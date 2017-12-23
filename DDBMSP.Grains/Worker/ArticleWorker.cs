using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using DDBMSP.Interfaces.Grains.Workers;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Worker
{
    [Reentrant]
    [StatelessWorker]
    public class ArticleWorker : Grain, IArticleWorker
    {
        private IDistributedHashTable<Guid, ArticleState> HashTable =>
            GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);

        public Task Create(Immutable<ArticleState> article) =>
            HashTable.Set(article.Value.Id.AsImmutable(), article.Value.AsImmutable());

        public Task CreateRange(Immutable<List<ArticleState>> articles) {
            var art = articles.Value;
            var dictArticlesRange = new Dictionary<Guid, ArticleState>(articles.Value.Count);

            foreach (var t in art) {
                dictArticlesRange.Add(t.Id, t);
            }
            return HashTable.SetRange(dictArticlesRange.AsImmutable());
        }
    }
}