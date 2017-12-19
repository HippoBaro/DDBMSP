using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Query;
using DDBMSP.Entities.User;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace DDBMSP.Common
{

    public enum ScriptType
    {
        QuerySelector,
        QueryAggregator
    }

    public class Globals
    {
        public Dictionary<Guid, ArticleState> Articles = null;
        public Dictionary<Guid, UserState> Users = null;
        public IEnumerable<object> TaskResult = null;
    }

    public class QueryScript
    {
        public Script<object> Selector { get; set; }
        public Script<object> Aggregator { get; set; }
    }

    public static class Evaluator
    {
        private static ScriptOptions ScriptOptions { get; } = ScriptOptions.Default
            .WithReferences(typeof(ArticleState).Assembly, typeof(System.Linq.IQueryable).Assembly,
                typeof(IEnumerable<>).Assembly, typeof(Guid).Assembly)
            .WithImports("DDBMSP.Entities.Article", "DDBMSP.Entities.User", "System.Linq", "System",
                "System.Collections.Generic");

        private static Dictionary<string, QueryScript> Queries { get; } = new Dictionary<string, QueryScript>();

        public static void CompileAndRegister(QueryDefinition query, string name) {

            var selector = CSharpScript.Create(query.SelectorLambda, ScriptOptions, typeof(Globals));
            selector.Compile();

            var castScript =
                CSharpScript.Create($"var Selected = TaskResult.Select(i=>({query.ReturnTypeName})i);",
                    ScriptOptions, typeof(Globals));

            var aggregator = castScript.ContinueWith<object>(query.AggregationLambda, ScriptOptions);
            aggregator.Compile();

            var scripts = new QueryScript {
                Aggregator = aggregator,
                Selector = selector
            };

            Queries.Add(name, scripts);
        }

        public static async Task<object> Execute(ScriptType type, string name, Globals context) {
            if (type == ScriptType.QuerySelector)
                return (await Queries[name].Selector.RunAsync(context)).ReturnValue;
            return (await Queries[name].Aggregator.RunAsync(context)).ReturnValue;
        }
    }
}