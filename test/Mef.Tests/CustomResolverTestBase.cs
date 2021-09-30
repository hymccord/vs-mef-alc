using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;

using Mef.Contracts;

using Microsoft.VisualStudio.Composition;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Shouldly;

using MefV1 = System.ComponentModel.Composition;

namespace Mef.Host.Tests
{
    public abstract class CustomResolverTestBase<T> where T : Resolver, new()
    {
        private readonly string _externalExtensionAssemblyPath;
        private readonly Resolver _resolver;

        public CustomResolverTestBase()
        {
            _externalExtensionAssemblyPath = Path.Combine(AppContext.BaseDirectory, "Extension1", "Mef.CorrectlyConfiguredExternalExtension.dll");
            _resolver = new T();
        }

        [TestMethod]
        public async Task V1DiscoveryWorks()
        {
            var discoveryService = new AttributedPartDiscoveryV1(_resolver);
            var result = await discoveryService.CreatePartsAsync(new[] { _externalExtensionAssemblyPath });

            result.Parts.Count.ShouldBe(2);
            foreach (var part in result.Parts)
            {
                part.Type.GetTypeInfo().IsAssignableTo(typeof(IExtension)).ShouldBeTrue();
            }
        }

        [TestMethod]
        public async Task V2DiscoveryWorks()
        {
            var discoveryService = new AttributedPartDiscovery(_resolver);
            var result = await discoveryService.CreatePartsAsync(new[] { _externalExtensionAssemblyPath });

            result.Parts.Count.ShouldBe(2);
            foreach (var part in result.Parts)
            {
                part.Type.GetTypeInfo().IsAssignableTo(typeof(IExtension)).ShouldBeTrue();
            }
        }

        [TestMethod]
        public async Task CombinedDiscoveryWorks()
        {
            var discoveryService = PartDiscovery.Combine(_resolver,
                new AttributedPartDiscovery(_resolver),
                new AttributedPartDiscoveryV1(_resolver));
            var result = await discoveryService.CreatePartsAsync(new[] { _externalExtensionAssemblyPath });

            result.Parts.Count.ShouldBe(3);
            foreach (var part in result.Parts)
            {
                part.Type.GetTypeInfo().IsAssignableTo(typeof(IExtension)).ShouldBeTrue();
            }
        }

        [Export, MefV1.Export]
        public class InternalPart : IExtension { }
    }
}
