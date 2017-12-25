using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using CommandLine;
using DDBMSP.Common;
using DDBMSP.Interfaces.Grains.Querier;
using Orleans;
using Orleans.Concurrency;

namespace DDBMSP.CLI.Interactive.Query
{
    [Verb("query exec", HelpText = "Create, compile and commit a new named query to the cluster")]
    public class ExecuteQuery
    {
        [Option('n', "name", Required = true, HelpText = "Name of the to-be-executed query")]
        public string Name { get; set; }
        
        [Option('p', "pipe", Required = false, HelpText = "Name of the variable to pipe result into")]
        public string VariableName { get; set; }

        public async Task<int> Run(CSharpRepl repl, IClusterClient client) {
            try {
                var querier = client.GetGrain<IGenericQuerier>(0);
                var res = await querier.Query(Name.AsImmutable());
                
                IFormatter formatter = new BinaryFormatter();  
                Stream stream = new MemoryStream(res.Value.Item2, false);  
                var obj = formatter.Deserialize(stream);
                stream.Close();

                if (VariableName != null)
                    await repl.AddToState(obj, VariableName, res.Value.Item1);
                else
                    await repl.Display(obj, res.Value.Item1);
                return 0;
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
                return 1;
            }
        }
    }
}
