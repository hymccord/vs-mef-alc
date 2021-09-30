using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mef.Host
{
    internal static class AssemblyUnification
    {
        internal static IImmutableSet<string> WellKnownAssemblyNames { get; private set; } =
            new[]
            {
                "Mef.Contracts",
                "System.ComponentModel.Composition",
                "System.Composition",
                "System.Composition.AttributedModel",
                "System.Composition.Convention",
                "System.Composition.Hosting",
                "System.Composition.Runtime",
                "System.Composition.TypedParts",
            }.ToImmutableHashSet();

        internal static void SetWellKnownAssemblies(IEnumerable<string> assemblies)
            => WellKnownAssemblyNames = assemblies.ToImmutableHashSet();
    }
}
