using System;
using System.Composition;

using Mef.Contracts;

using MefV1 = System.ComponentModel.Composition;

namespace Mef.IncorrectlyConfiguredExternalExtension
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
    public class OnlyV2Part : IExtension { }
}
