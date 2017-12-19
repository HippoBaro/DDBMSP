using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DDBMSP.Common;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Query;
using DDBMSP.Interfaces.Grains.Core.DistributedHashTable;
using DDBMSP.Interfaces.Grains.Querier;
using Newtonsoft.Json;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.Grains.Querier
{
    [StatelessWorker]
    [Reentrant]
    public class GenericQuerier : Grain, IGenericQuerier
    {
        public async Task<dynamic> Execute() {
            var articles = GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);

            var query = new QueryDefinition {
                AggregationLambda = "Selected.Sum()",
                SelectorLambda = "Articles.Count(pair => !string.IsNullOrEmpty(pair.Value.Title))",
                ReturnTypeName = "long"
            };
            
            
            var t = Stopwatch.StartNew();
            Evaluator.CompileAndRegister(query, "test");
            Console.WriteLine($"Compile: {t.Elapsed:g}");
            t.Restart();

            try {
                var str = (await articles.Execute(query.AsImmutable())).Value;
                Console.WriteLine($"Query: {t.Elapsed:g}");
                var count = JsonConvert.DeserializeObject<int>(str);

                Console.WriteLine(count);

            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            return null;
        }
    }
}