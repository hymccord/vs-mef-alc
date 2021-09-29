using System;
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
            Console.WriteLine($@"   {nameof(ExternalExtension)} created
        My directory is: {Directory.GetParent(assembly.Location).FullName}
        Using Newtonsoft.Json v{jsonAssemblyName.Version}
        {JsonConvert.ToString(DateTime.Now)}");

        }
    }
}
