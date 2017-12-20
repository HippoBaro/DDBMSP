using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Query;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using DDBMSP.Interfaces.Grains.Querier;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Querier
{
    [StatelessWorker]
    [Reentrant]
    public class GenericQuerier : Grain, IGenericQuerier
    {
        public async Task<Immutable<object>> Query(Immutable<string> queryName) {
            var def = await GrainFactory.GetGrain<IQueryRepository>(0).GetQueryDefinition(queryName);
            var ret = await GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0)
                .Query(def);
            
            Console.WriteLine("Returning query's result");
            var res = ((object)ret.Value).AsImmutable();
            Console.WriteLine("Returning query's result 2");
            return res;
        }

        public Task<Immutable<QueryDefinition>> GetQueryDefinition(Immutable<string> queryName) => GrainFactory.GetGrain<IQueryRepository>(0).GetQueryDefinition(queryName);
    }
}