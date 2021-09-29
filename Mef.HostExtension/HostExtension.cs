using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;

using Mef.Contracts;

using Newtonsoft.Json;

namespace Mef.HostExtension
{
    [Export(typeof(IExtension))]
    public class HostExtension : IExtension
    {
        private readonly IImport _import;

        [ImportingConstructor]
        public HostExtension(IImport import)
        {
            _import = import;

            var assembly = Assembly.GetExecutingAssembly();
            var jsonAssembly = typeof(JsonConvert).Assembly;
            Console.WriteLine($@"   {nameof(HostExtension)} created
        My directory is: {Directory.GetParent(assembly.Location).FullName}
        Using Newtonsoft.Json {jsonAssembly.FullName}
        {JsonConvert.ToString(DateTime.Now)}");

        }
    }
}
