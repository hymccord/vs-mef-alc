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
        internal static readonly IImmutableSet<string> WellKnownAssemblyNames =
         new[]
         {
            "Mef.Contracts",
            "System.ComponentModel.Composition"
         }.ToImmutableHashSet();
    }
}
