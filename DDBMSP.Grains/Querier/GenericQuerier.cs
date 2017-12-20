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
        public async Task<Immutable<dynamic>> Query(Immutable<string> queryName) {
            var def = await GrainFactory.GetGrain<IQueryRepository>(0).GetQueryDefinition(queryName);
            var ret = await GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0)
                .Query(def);
            
            Console.WriteLine("Returning query's result");
            
            Console.WriteLine(ret.Value.GetType().Name);
            
            var t = new Dictionary<Guid, ArticleState>();

            var tt = t.Take(1);

            if (ret.Value.GetType().Name.StartsWith("Dictionary")) {
                Console.WriteLine("It's a dictionnary");
                var ret2 = ret.Value.Take(1).AsImmutable();
                Console.WriteLine(ret2.Value.GetType().Name);
                return ret2;
            }
            
            return ret;
        }
    }
}