using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Query;
using DDBMSP.Entities.User;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace DDBMSP.Common.QueryEngine
{
    public enum ScriptType
    {
        QuerySelector,
        QueryAggregator
    }

    public class QueryContext
    {
        public IEnumerable<object> __TaskResult = null;
        public IEnumerable<dynamic> __Elements = null;
    }

    public class QueryScript
    {
        public ScriptRunner<object> Selector { get; set; }
        public ScriptRunner<object> Aggregator { get; set; }
    }

    public static class QueryEngine
    {
        private static ScriptOptions ScriptOptions = ScriptOptions.Default
            .WithReferences(typeof(ArticleState).Assembly, typeof(IQueryable).Assembly,
                typeof(IEnumerable<>).Assembly, typeof(Guid).Assembly)
            .WithImports("DDBMSP.Entities.Article", "DDBMSP.Entities.User", "DDBMSP.Entities.UserActivity", "System.Linq", "System",
                "System.Collections.Generic").WithEmitDebugInformation(false);

        private static Dictionary<string, QueryScript> Queries = new Dictionary<string, QueryScript>();

        public static void CompileAndRegister(QueryDefinition queryDefinition) {
            try {
                if (Queries.ContainsKey(queryDefinition.Name))
                    throw new Exception($"Query \"{queryDefinition.Name}\" already exists.");
                
                var selector = CSharpScript.Create($"var Elements = (IEnumerable<{queryDefinition.TargetRessource}State>)__Elements;",
                        ScriptOptions, typeof(QueryContext))
                    .ContinueWith<object>(queryDefinition.SelectorLambda, ScriptOptions);
                selector.Compile();
                
                var aggregator = CSharpScript.Create(
                        $"var Selected = __TaskResult.Select(i=>({queryDefinition.ReturnTypeName})i);",
                        ScriptOptions, typeof(QueryContext))
                    .ContinueWith<object>(queryDefinition.AggregationLambda, ScriptOptions);
                aggregator.Compile();
                
                var query = new QueryScript();
                Console.WriteLine("compiling selection script");
                query.Aggregator = aggregator.CreateDelegate();
                Console.WriteLine("compiling aggregation script");
                query.Selector = selector.CreateDelegate();
                Console.WriteLine("compiling done");

                Queries.Add(queryDefinition.Name, query);
            }
            catch (Exception e) {
                throw new Exception(e.Message);
            }
        }

        public static async Task<object> Execute(ScriptType type, QueryDefinition queryDefinition,
            QueryContext context) {
            try {
                if (!Queries.ContainsKey(queryDefinition.Name))
                    CompileAndRegister(queryDefinition);

                dynamic ret;
                if (type == ScriptType.QuerySelector) {
                    Console.WriteLine("Running selection script");
                    ret = await Queries[queryDefinition.Name].Selector.Invoke(context);
                }
                else {
                    Console.WriteLine("Running aggregation script");
                    ret = await Queries[queryDefinition.Name].Aggregator.Invoke(context);
                }

                return ret;
            }
            catch (Exception e) {
                Console.WriteLine("Throw!");
                throw new Exception(e.Message);
            }
        }
    }
}