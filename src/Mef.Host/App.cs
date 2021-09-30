using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Mef.Contracts;

using Microsoft;
using Microsoft.VisualStudio.Composition;

namespace Mef.Host
{
    public class App
    {
        private string _hostExtensionDllPath;
        private string _externalExtensionDllPath;
        private string _externalExtensionV2DllPath;
        public App()
        {
            bool published = AppContext.BaseDirectory.Contains("publish");
            _hostExtensionDllPath = Path.Combine(AppContext.BaseDirectory, "Mef.HostExtension.dll");
            // <project>/bin/<config>/<tfm>/ -> ../../../..

            var path = published
                ? Path.Combine("../../../../..", "Mef.ExternalExtension/bin/Debug/net5.0/publish")
                : Path.Combine("../../../..", "Mef.ExternalExtension/bin/Debug/net5.0");
            _externalExtensionDllPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, path, "Mef.ExternalExtension.dll"));
            _externalExtensionV2DllPath = _externalExtensionDllPath.Replace("ExternalExtension", "ExternalExtensionV2");
        }

        public async Task Run(Resolver resolver)
        {
            var logger = new IndentedTextWriter(Console.Out, "\t");

            var discovery = PartDiscovery.Combine(resolver,
                new AttributedPartDiscovery(resolver),
                new AttributedPartDiscoveryV1(resolver)); // ".NET MEF" attributes (System.ComponentModel.Composition)

            var catalog = ComposableCatalog.Create(resolver)
                .AddParts(await discovery.CreatePartsAsync(Assembly.GetExecutingAssembly()))
                .AddParts(await discovery.CreatePartsAsync(new string[]
                    {
                        _externalExtensionDllPath,
                        _externalExtensionV2DllPath,
                        _hostExtensionDllPath,
                    }));

            Console.ForegroundColor = ConsoleColor.Green;
            logger.Indent++;
            logger.WriteLine();
            await logger.WriteLineAsync("Part IDs found");
            logger.Indent++;
            foreach(var id in catalog.Parts.Select(c => c.Id))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                await logger.WriteLineAsync(id);
            }
            Console.ForegroundColor = ConsoleColor.White;

            var exportProvider = CompositionConfiguration
                .Create(catalog)
                .CreateExportProviderFactory()
                .CreateExportProvider();

            IExtension[] extensions = exportProvider
                .GetExportedValues<IExtension>()
                .ToArray();
        }
    }

}
