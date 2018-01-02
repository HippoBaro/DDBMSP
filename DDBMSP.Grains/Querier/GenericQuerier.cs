using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Query;
using DDBMSP.Entities.User;
using DDBMSP.Entities.UserActivity;
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
        private async Task<Immutable<byte[]>> Query<TRessourceType>(Immutable<QueryDefinition> queryDef) {
            try {
                var ret = await GrainFactory.GetGrain<IDistributedHashTable<Guid, TRessourceType>>(0).Query(queryDef);
                IFormatter formatter = new BinaryFormatter();  
                var stream = new MemoryStream(5000000);
                formatter.Serialize(stream, ret.Value);
                stream.Close();
                
                Console.WriteLine("Returning query's result");
                return stream.GetBuffer().AsImmutable();
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        public async Task<Immutable<Tuple<QueryDefinition, byte[]>>> Query(Immutable<string> queryName) {
            var def = await GrainFactory.GetGrain<IQueryRepository>(0).GetQueryDefinition(queryName);

            Immutable<byte[]> res;
            
            switch (def.Value.TargetRessource) {
                case "ArticleState":
                    res = await Query<ArticleState>(def);
                    break;
                case "UserState":
                    res = await Query<UserState>(def);
                    break;
                case "List<UserActivityState>":
                    res = await Query<List<UserActivityState>>(def);
                    break;
                default:
                    throw new Exception("Unknown ressource type");
            }
            return new Tuple<QueryDefinition, byte[]>(def.Value, res.Value).AsImmutable();
        }

        public Task<Immutable<QueryDefinition>> GetQueryDefinition(Immutable<string> queryName) => GrainFactory.GetGrain<IQueryRepository>(0).GetQueryDefinition(queryName);
    }
}