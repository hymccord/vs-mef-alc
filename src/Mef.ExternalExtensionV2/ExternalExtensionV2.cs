using System;
using System.CodeDom.Compiler;
using System.Composition;
using System.IO;
using System.Reflection;

using Mef.Contracts;

using Newtonsoft.Json;

namespace Mef.ExternalExtensionV2
{
    //[Export(typeof(IExtension))]
    public class ExternalExtensionV2 : IExtension
    {
        private readonly IImport _import;

        [ImportingConstructor]
        public ExternalExtensionV2(IImport import)
        {
            _import = import;

            var assembly = Assembly.GetExecutingAssembly();
            var jsonAssembly = typeof(JsonConvert).Assembly;
            var jsonAssemblyName = AssemblyName.GetAssemblyName(jsonAssembly.Location);

            var writer = new IndentedTextWriter(Console.Out, "\t") { Indent = 1 };
            writer.WriteLine();
            writer.WriteLine($"{nameof(ExternalExtensionV2)} created");
            writer.Indent++;
            writer.WriteLine($"My directory is: {Directory.GetParent(assembly.Location).FullName}");
            writer.WriteLine($"Using Newtonsoft.Json v{jsonAssemblyName.Version}");
            writer.WriteLine($"JsonConvert.ToString(DateTime.Now) -> {JsonConvert.ToString(DateTime.Now)}");
        }
    }
}
