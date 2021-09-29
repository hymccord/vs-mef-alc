using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Mef.Host
{
    // Simple ALC. Taken from MSBuildLoadContext in dotnet/msbuild
    public class IsolatedLoadContext : AssemblyLoadContext
    {
        private readonly string _directory;

        public IsolatedLoadContext(string assemblyPath)
            : base($"Isolated plugin {assemblyPath}")
        {
            _directory = Directory.GetParent(assemblyPath)!.FullName;
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (AssemblyUnification.WellKnownAssemblyNames.Contains(assemblyName.Name!))
            {
                return null;
            }

            foreach (var cultureSubfolder in string.IsNullOrEmpty(assemblyName.CultureName)
                // If no culture is specified, attempt to load directly from
                // the known dependency paths.
                ? new[] { string.Empty }
                // Search for satellite assemblies in culture subdirectories
                // of the assembly search directories, but fall back to the
                // bare search directory if that fails.
                : new[] { assemblyName.CultureName, string.Empty })
            {
                var candidatePath = Path.Combine(_directory,
                    cultureSubfolder,
                    $"{assemblyName.Name}.dll");

                if (!File.Exists(candidatePath))
                {
                    continue;
                }

                AssemblyName candidateAssemblyName = GetAssemblyName(candidatePath);
                if (candidateAssemblyName.Version != assemblyName.Version)
                {
                    continue;
                }

                return LoadFromAssemblyPath(candidatePath);
            }

            // If under appcontext, load into default ALC
            var assemblyNameInExeDirectory = Path.Combine(AppContext.BaseDirectory, $"{assemblyName.Name}.dll");
            if (File.Exists(assemblyNameInExeDirectory))
            {
                return Default.LoadFromAssemblyPath(assemblyNameInExeDirectory);
            }

            return null;
        }
    }

    internal static class AssemblyUnification
    {
        internal static readonly ImmutableHashSet<string> WellKnownAssemblyNames =
         new[]
         {
                    "Mef.Contracts",
                    "System.ComponentModel.Composition"
         }.ToImmutableHashSet();
    }
}
