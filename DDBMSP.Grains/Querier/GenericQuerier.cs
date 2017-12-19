using System;
using System.Diagnostics;
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
        public async Task<dynamic> Execute() {
            var articles = GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0);

            var query = new QueryDefinition {
                AggregationLambda = "Selected.Sum()",
                SelectorLambda = "Articles.Count(pair => !string.IsNullOrEmpty(pair.Value.Title))",
                ReturnTypeName = "int"
            };

            Guid toto;
            
            var query2 = new QueryDefinition {
                AggregationLambda = "Selected.SelectMany(dict => dict).ToDictionary(pair => pair.Key, pair => pair.Value)",
                SelectorLambda = "Articles.Where(pair => pair.Value.Author.Name.Contains(\"Mira\"))",
                ReturnTypeName = "IEnumerable<KeyValuePair<Guid, ArticleState>>"
            };
            
            var t = Stopwatch.StartNew();
            Evaluator.CompileAndRegister(query, "test");
            Console.WriteLine($"Compile: {t.Elapsed:g}");
            t.Restart();
            
            Evaluator.CompileAndRegister(query2, "test2");
            Console.WriteLine($"Compile: {t.Elapsed:g}");
            t.Restart();

            try {

                var test = await GrainFactory.GetGrain<IDistributedHashTable<Guid, ArticleState>>(0)
                    .Execute(query2.AsImmutable());
                
                //var str = (await articles.Execute(query.AsImmutable())).Value;
                Console.WriteLine($"Query: {t.ElapsedMilliseconds}ms");
                //var count = JsonConvert.DeserializeObject<int>(str);

                Console.WriteLine(test.Value.Count);

            }
            catch (Exception e) {
                Console.WriteLine(e);
                throw;
            }
            return null;
        }
    }
}