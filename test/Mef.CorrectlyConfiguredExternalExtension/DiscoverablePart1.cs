using System;
using System.Composition;

using Mef.Contracts;

using MefV1 = System.ComponentModel.Composition;

namespace Mef.AssemblyPartsTests
{
    [Export(typeof(IExtension)), MefV1.Export(typeof(IExtension))]
    public class DiscoverablePart : IExtension
    {
        [ImportingConstructor]
        [MefV1.ImportingConstructor]
        public DiscoverablePart(IImport import)
        {

        }
    }

    [MefV1.Export(typeof(IExtension))]
    public class OnlyV1Part : IExtension { }

    [Export(typeof(IExtension))]
    public class OnlyV2Part : IExtension {  }

    //[Export(typeof(IExtension)), MefV1.Export(typeof(IExtension))]
    //public class DiscoverableExtension : IExtension
    //{

    //}

    //[Export(typeof(IExtension)),  MefV1.Export(typeof(IExtension))]
    //public class DiscoverableExtensionWithImport : IExtension
    //{
    //    private readonly IImport _import;

    //    [MefV1.ImportingConstructor]
    //    public DiscoverableExtensionWithImport(IImport import)
    //    {
    //        _import = import;
    //    }
    //}

    //[PartNotDiscoverable, MefV1.PartNotDiscoverable]
    //[Export, MefV1.Export]
    //public class NonDiscoverablePart
    //{
    //}

    //[MefV1.PartNotDiscoverable]
    //[MefV1.Export]
    //public class NonDiscoverablePartV1
    //{
    //}

    //[PartNotDiscoverable]
    //[Export]
    //public class NonDiscoverablePartV2
    //{
    //}

    //public class NonPart
    //{

    //}
}
