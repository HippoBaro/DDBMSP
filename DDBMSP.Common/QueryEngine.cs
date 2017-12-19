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

    public class QueryContext
    {
        public Dictionary<Guid, ArticleState> Articles = null;
        public Dictionary<Guid, UserState> Users = null;
        public IEnumerable<object> TaskResult = null;
    }

    public class QueryScript
    {
        public ScriptRunner<object> Selector { get; set; }
        public ScriptRunner<object> Aggregator { get; set; }
    }

    public static class QueryEngine
    {
        private static Dictionary<string, QueryScript> Queries { get; } = new Dictionary<string, QueryScript>();

        private static ScriptOptions ScriptOptions { get; } = ScriptOptions.Default
            .WithReferences(typeof(ArticleState).Assembly, typeof(System.Linq.IQueryable).Assembly,
                typeof(IEnumerable<>).Assembly, typeof(Guid).Assembly)
            .WithImports("DDBMSP.Entities.Article", "DDBMSP.Entities.User", "System.Linq", "System",
                "System.Collections.Generic").WithEmitDebugInformation(false);

        public static void CompileAndRegister(QueryDefinition queryDefinition) {
            if (Queries.ContainsKey(queryDefinition.Name))
                throw new Exception($"Query \"{queryDefinition.Name}\" already exists.");
            
            var selector = CSharpScript.Create(queryDefinition.SelectorLambda, ScriptOptions, typeof(QueryContext));
            selector.Compile();

            var aggregator = CSharpScript.Create($"var Selected = TaskResult.Select(i=>({queryDefinition.ReturnTypeName})i);",
                ScriptOptions, typeof(QueryContext)).ContinueWith<object>(queryDefinition.AggregationLambda, ScriptOptions);
            aggregator.Compile();
            
            Queries.Add(queryDefinition.Name, new QueryScript {
                Aggregator = aggregator.CreateDelegate(),
                Selector = selector.CreateDelegate()
            });
        }

        public static async Task<object> Execute(ScriptType type, string name, QueryContext context) {
            if (type == ScriptType.QuerySelector)
                return await Queries[name].Selector.Invoke(context);
            return await Queries[name].Aggregator.Invoke(context);
        }
    }
}