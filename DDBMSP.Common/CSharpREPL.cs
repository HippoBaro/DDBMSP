using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DDBMSP.Entities.Article;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.Hosting;

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
            .WithReferences(typeof(ArticleState).Assembly, typeof(System.Linq.IQueryable).Assembly,
                typeof(IEnumerable<>).Assembly, typeof(Guid).Assembly)
            .WithImports("DDBMSP.Entities.Article", "DDBMSP.Entities.User", "System.Linq", "System",
                "System.Collections.Generic").WithEmitDebugInformation(false);
        
        public InteractiveAssemblyLoader InteractiveAssemblyLoader { get; set; } = new InteractiveAssemblyLoader();
        public ScriptState<dynamic> ScriptState { get; set; }

        public Context ScriptContext = new Context();

        public CSharpRepl() {
            var script = CSharpScript.Create<dynamic>("true", ScriptOptions, typeof(Context));
            ScriptState = script.RunAsync(ScriptContext).Result;
        }

        public async Task<dynamic> REPL(string line) {
            return (ScriptState = await ScriptState.ContinueWithAsync<dynamic>(line, ScriptOptions)).ReturnValue;
        }
        
        public async Task AddToState(dynamic obj, string name) {
            Type type = obj.GetType();
            ScriptContext.__Results.Add(obj);

            if (ScriptState.Variables.Any(variable => variable.Name == name && variable.Type == type)) {
                ScriptState = await ScriptState.ContinueWithAsync<dynamic>(
                    $"{name} = ({type.Name}) __Results[__Counter++];", ScriptOptions);
            }
            else {
                ScriptState = await ScriptState.ContinueWithAsync<dynamic>(
                    $"var {name} = Convert.ChangeType(__Results[__Counter++], Type.GetType(\"{type.AssemblyQualifiedName}\"));", ScriptOptions);
            }
        }
    }
}