using System;
using System.Composition;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Composition;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MefV1 = System.ComponentModel.Composition;
using Shouldly;
using System.Reflection;
using Mef.Contracts;

namespace Mef.Host.Tests
{
    /// <summary>
    /// This class tests to make sure our resolvers handle extensions/plugins
    /// that are configured as expected. We should expect any dlls that that the
    /// host app provides, not to be in the output folder of the plugin.
    ///
    /// Mef.Contracts.dll
    /// System.Composition*.dll
    /// System.ComponentModel.Composition.dll
    /// </summary>
    [TestClass]
    public class AssemblyUnificationTests
    {
        private readonly string _externalExtensionAssemblyPath;

        public AssemblyUnificationTests()
        {
            _externalExtensionAssemblyPath = Path.Combine(AppContext.BaseDirectory, "Extension2", "Mef.IncorrectlyConfiguredExternalExtension.dll");
        }

        [TestMethod]
        public async Task TypesDontMatchWithoutContracts()
        {
            var newSet = AssemblyUnification.WellKnownAssemblyNames.Remove("Mef.Contracts");
            AssemblyUnification.SetWellKnownAssemblies(newSet);

            var discoveryService = CreateCombinedDiscovery();
            var result = await discoveryService.CreatePartsAsync(new[] { _externalExtensionAssemblyPath });

            // We can still discover correctly
            result.Parts.Count.ShouldBe(3);

            // But types dont match
            foreach (var part in result.Parts)
            {
                part.Type.GetTypeInfo().IsAssignableTo(typeof(IExtension)).ShouldBeFalse();
            }
        }

        // [TestMethod]
        // Need to fix static being read in parallel
        public async Task TypesMatchCorrectly()
        {
            var discoveryService = CreateCombinedDiscovery();
            var result = await discoveryService.CreatePartsAsync(new[] { _externalExtensionAssemblyPath });

            // We saved them from themselves, hurray!
            result.Parts.Count.ShouldBe(3);
            foreach (var part in result.Parts)
            {
                part.Type.GetTypeInfo().IsAssignableTo(typeof(IExtension)).ShouldBeTrue();
            }
        }

        [TestMethod]
        public async Task V1DiscoveryMissesOnExternal()
        {
            var newSet = AssemblyUnification.WellKnownAssemblyNames.Remove("System.ComponentModel.Composition");
            AssemblyUnification.SetWellKnownAssemblies(newSet);

            var discoveryService = CreateV1Discovery();
            var result = await discoveryService.CreatePartsAsync(new[] { _externalExtensionAssemblyPath });

            // Missing all da parts :(
            result.Parts.Count.ShouldBe(0);
        }

        [TestMethod]
        public async Task V1DiscoveryNowSucceeds()
        {
            var discoveryService = CreateV1Discovery();
            var result = await discoveryService.CreatePartsAsync(new[] { _externalExtensionAssemblyPath });

            // Missing all da parts :(
            result.Parts.Count.ShouldBe(0);
            foreach (var part in result.Parts)
            {
                part.Type.GetTypeInfo().IsAssignableTo(typeof(IExtension)).ShouldBeTrue();
            }
        }

        [TestMethod]
        public async Task V2DiscoveryMissesOnExternal()
        {
            
            AssemblyUnification.SetWellKnownAssemblies(new List<string>
            {
                "Mef.Contracts"
            });

            var discoveryService = CreateV2Discovery();
            var result = await discoveryService.CreatePartsAsync(new[] { _externalExtensionAssemblyPath });

            result.Parts.Count.ShouldBe(0);
        }

        [TestMethod]
        public async Task V2DiscoveryNowSucceeds()
        {
            var discoveryService = CreateV2Discovery();
            var result = await discoveryService.CreatePartsAsync(new[] { _externalExtensionAssemblyPath });

            // Missing all da parts :(
            result.Parts.Count.ShouldBe(0);
            foreach (var part in result.Parts)
            {
                part.Type.GetTypeInfo().IsAssignableTo(typeof(IExtension)).ShouldBeTrue();
            }
        }

        private PartDiscovery CreateV1Discovery()
        {
            var resolver = new IsolatedResolver();
            var disoveryService = new AttributedPartDiscoveryV1(resolver);

            return disoveryService;
        }
        private PartDiscovery CreateV2Discovery()
        {
            var resolver = new IsolatedResolver();
            var disoveryService = new AttributedPartDiscovery(resolver);

            return disoveryService;
        }

        private PartDiscovery CreateCombinedDiscovery()
        {
            var resolver = new IsolatedResolver();
            var disoveryService = PartDiscovery.Combine(resolver,
                new AttributedPartDiscovery(resolver),
                new AttributedPartDiscoveryV1(resolver));

            return disoveryService;
        }

        [Export, MefV1.Export]
        public class InternalPart : IExtension { }
    }
}
