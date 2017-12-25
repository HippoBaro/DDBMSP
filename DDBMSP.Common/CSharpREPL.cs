using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using DDBMSP.Entities.Query;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace DDBMSP.Common
{
    public class CSharpRepl
    {
        public class Context
        {
            public int __Counter { get; set; } = 0;
            public List<dynamic> __Results { get; } = new List<dynamic>();
        }

        private static ScriptOptions ScriptOptions { get; } = ScriptOptions.Default
            .WithReferences(typeof(ArticleState).Assembly, typeof(IQueryable).Assembly,
                typeof(IEnumerable<>).Assembly, typeof(Guid).Assembly, typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly)
            .WithImports("DDBMSP.Entities.Article", "DDBMSP.Entities.User", "DDBMSP.Entities.UserActivity", "System.Linq", "System",
                "System.Collections.Generic", "Microsoft.CSharp").WithEmitDebugInformation(false);
        
        public ScriptState<dynamic> ScriptState { get; set; }
        public readonly Context ScriptContext = new Context();

        public CSharpRepl() {
            var script = CSharpScript.Create<dynamic>("true", ScriptOptions, typeof(Context));
            ScriptState = script.RunAsync(ScriptContext).Result;
        }

        public async Task<dynamic> Evaluate(string line) => (ScriptState = await ScriptState.ContinueWithAsync<dynamic>(line, ScriptOptions)).ReturnValue;

        public async Task AddToState(dynamic obj, string name, QueryDefinition query) {
            
            Type type = obj.GetType();
            ScriptContext.__Results.Add(obj);

            if (ScriptState.Variables.Any(variable => variable.Name == name && variable.Type == type)) {
                ScriptState = await ScriptState.ContinueWithAsync<dynamic>(
                    $"{name} = ({query.ReturnTypeName}) __Results[__Counter++];", ScriptOptions);
            }
            else {
                ScriptState = await ScriptState.ContinueWithAsync<dynamic>(
                    $"var {name} = Convert.ChangeType(__Results[__Counter++], Type.GetType(\"{type.AssemblyQualifiedName}\"));",
                    ScriptOptions);
            }
        }

        public async Task Display(dynamic obj, QueryDefinition query) {
            ScriptContext.__Results.Add(obj);
            ScriptState = await ScriptState.ContinueWithAsync<dynamic>(
                $"Console.WriteLine((({query.ReturnTypeName}) __Results[__Counter++]).ToString())", ScriptOptions);
        }
    }
}