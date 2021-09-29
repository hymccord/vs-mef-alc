using System;
using System.CodeDom.Compiler;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

using Mef.Contracts;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mef.ExternalExtension
{
    [Export(typeof(IExtension))]
    public class ExternalExtension : IExtension
    {
        private readonly IImport _import;

        [ImportingConstructor]
        public ExternalExtension(IImport import)
        {
            _import = import;

            var assembly = Assembly.GetExecutingAssembly();
            var jsonAssembly = typeof(JsonConvert).Assembly;
            var jsonAssemblyName = AssemblyName.GetAssemblyName(jsonAssembly.Location);

            var writer = new IndentedTextWriter(Console.Out, "\t") { Indent = 1 };
                writer.WriteLine();
            writer.WriteLine($"{nameof(ExternalExtension)} created");
            writer.Indent++;
            writer.WriteLine($"My directory is: {Directory.GetParent(assembly.Location).FullName}");
            writer.WriteLine($"Using Newtonsoft.Json v{jsonAssemblyName.Version}");
            writer.WriteLine($"JsonConvert.ToString(DateTime.Now) -> {JsonConvert.ToString(DateTime.Now)}");
        }
    }
}
