using System;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Mef.Contracts;

using Microsoft.VisualStudio.Composition;

namespace Mef.Host
{
    internal class Program
    {

        static async Task Main(string[] args)
        {
            await new App().Run();
        }
    }

    public class App
    {
        private string _hostExtensionDllPath;
        private string _externalExtensionDllPath;
        public App()
        {
            bool published = AppContext.BaseDirectory.Contains("publish");
            _hostExtensionDllPath = Path.Combine(AppContext.BaseDirectory, "Mef.HostExtension.dll");
            // <project>/bin/<config>/<tfm>/ -> ../../../..

            var path = published
                ? Path.Combine("../../../../..", "Mef.ExternalExtension/bin/Debug/net5.0/publish")
                : Path.Combine("../../../..", "Mef.ExternalExtension/bin/Debug/net5.0");
            _externalExtensionDllPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path, "Mef.ExternalExtension.dll"));
        }

        public async Task Run()
        {
            // Pick flavor of Resolver
            Resolver resolver;
            resolver = new IsolatedResolver();
            //resolver = new PluginResolver();

            var discovery = PartDiscovery.Combine(resolver,
                new AttributedPartDiscoveryV1(resolver)); // ".NET MEF" attributes (System.ComponentModel.Composition)

            var catalog = ComposableCatalog.Create(resolver)
                .AddParts(await discovery.CreatePartsAsync(Assembly.GetExecutingAssembly()))
                .AddParts(await discovery.CreatePartsAsync(new string[]
                    {
                        _externalExtensionDllPath,
                        _hostExtensionDllPath,
                    }));

            foreach(var id in catalog.Parts.Select(c => c.Id))
            {
                Console.WriteLine($"Part id: {id}");
            }

            var exportProvider = CompositionConfiguration
                .Create(catalog)
                .CreateExportProviderFactory()
                .CreateExportProvider();

            IExtension[] extensions = exportProvider
                .GetExportedValues<IExtension>()
                .ToArray();
        }
    }

    [Export(typeof(IImport))]
    public class Import : IImport { }

}
