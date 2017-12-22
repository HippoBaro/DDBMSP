using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Query;
using DDBMSP.Entities.User;
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

        public async Task<Immutable<byte[]>> Query(Immutable<string> queryName) {
            var def = await GrainFactory.GetGrain<IQueryRepository>(0).GetQueryDefinition(queryName);

            switch (def.Value.TargetRessource) {
                case "Article":
                    return await Query<ArticleState>(def);
                case "User":
                    return await Query<UserState>(def);
                default:
                    throw new Exception("Unknown ressource type");
            }
        }

        public Task<Immutable<QueryDefinition>> GetQueryDefinition(Immutable<string> queryName) => GrainFactory.GetGrain<IQueryRepository>(0).GetQueryDefinition(queryName);
    }
}