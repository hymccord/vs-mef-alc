using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using Mef.Contracts;

using Microsoft.VisualStudio.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Mef.Host.Tests
{
    [TestClass]
    public class UnitTest1
    {
        private readonly string _pluginDir;
        private readonly string _pluginFullPath;
        private readonly AssemblyName _assemblyName;

        public UnitTest1()
        {
            _pluginDir = Environment.ExpandEnvironmentVariables($@"%USERPROFILE%\source\repos\net5test\ALC\mef.plugin\bin\Debug\net5.0\publish\deeper");
            _pluginFullPath = Path.Combine(_pluginDir, "mef.plugin.dll");
            _assemblyName = Utilities.GetAssemblyNameWithCodebasePath(_pluginFullPath);
        }

        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [TestMethod]
        public void IsolatedALC(bool useAppContextDirectory, bool useSimpleAssemblyName)
        {
            var alc = new IsolatedLoadContext(useAppContextDirectory ? _pluginDir : _pluginFullPath );
            //var asm = alc.LoadFromAssemblyName(useSimpleAssemblyName ? new AssemblyName("mef.plugin") : _assemblyName);
            var asm = alc.LoadFromAssemblyPath(_pluginFullPath);
            var types = asm.GetTypes().ToList();
            Assert.AreEqual(1, types.Where(t => Utilities.IsExport(t)).Count());
        }

        [DataRow(false, false)]
        [DataRow(false, true)]
        [DataRow(true, false)]
        [DataRow(true, true)]
        [TestMethod]
        public void PluginALC(bool useAppContextDirectory, bool useSimpleAssemblyName)
        {
            var alc = new PluginLoadContext(useAppContextDirectory ? _pluginDir : _pluginFullPath);
            //var asm = alc.LoadFromAssemblyName(useSimpleAssemblyName ? new AssemblyName("mef.plugin") : _assemblyName);
            var asm = alc.LoadFromAssemblyPath(_pluginFullPath);
            var types = asm.GetTypes().ToList();
            Assert.AreEqual(1, types.Where(t => Utilities.IsExport(t)).Count());
        }

        [TestMethod]
        public async Task VsMefIsolatedALC()
        {
            var isolatedResolver = new IsolatedResolver();
            var discovery = PartDiscovery.Combine(isolatedResolver,
                new AttributedPartDiscoveryV1(isolatedResolver)); // ".NET MEF" attributes (System.ComponentModel.Composition)

            var catalog = ComposableCatalog.Create(isolatedResolver)
                .AddParts(await discovery.CreatePartsAsync(new string[] { _pluginFullPath }));
            var config = CompositionConfiguration.Create(catalog);
            var epf = config.CreateExportProviderFactory();
            var exportProvider = epf.CreateExportProvider();
            Assert.AreEqual(1, exportProvider.GetExports<IExtension>().Count());
        }

        [TestMethod]
        public async Task VsMefPluginALC()
        {
            // Prepare part discovery to support both flavors of MEF attributes.
            var isolatedResolver = new PluginResolver();
            var discovery = PartDiscovery.Combine(isolatedResolver,
                new AttributedPartDiscoveryV1(isolatedResolver)); // ".NET MEF" attributes (System.ComponentModel.Composition)

            // Build up a catalog of MEF parts
            var catalog = ComposableCatalog.Create(isolatedResolver)
                //.AddParts(await discovery.CreatePartsAsync(Assembly.GetExecutingAssembly()))
                .AddParts(await discovery.CreatePartsAsync(new string[] { _pluginFullPath }))
                .WithCompositionService(); // Makes an ICompositionService export available to MEF parts to import

            // Assemble the parts into a valid graph.
            var config = CompositionConfiguration.Create(catalog);

            // Prepare an ExportProvider factory based on this graph.
            var epf = config.CreateExportProviderFactory();

            // Create an export provider, which represents a unique container of values.
            // You can create as many of these as you want, but typically an app needs just one.
            var exportProvider = epf.CreateExportProvider();

            // Obtain our first exported value
            Assert.AreEqual(1, exportProvider.GetExports<IExtension>().Count());
        }
    }
}
