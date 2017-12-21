using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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
    public class GenericQuerier<TRessource, TResult> : Grain, IGenericQuerier<TRessource, TResult>
    {
        public async Task<Immutable<byte[]>> Query(Immutable<string> queryName) {
            var def = await GrainFactory.GetGrain<IQueryRepository>(0).GetQueryDefinition(queryName);
            var ret = await GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0)
                .Query(def);
            
            Console.WriteLine("Serialiazing query's result");

            try {
                IFormatter formatter = new BinaryFormatter();  
                var stream = new MemoryStream(5000000);  
                formatter.Serialize(stream, (TResult)ret.Value);  
                stream.Close();
                
                Console.WriteLine("Returning query's result");
                return stream.GetBuffer().AsImmutable();
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public Task<Immutable<QueryDefinition>> GetQueryDefinition(Immutable<string> queryName) => GrainFactory.GetGrain<IQueryRepository>(0).GetQueryDefinition(queryName);
    }
}