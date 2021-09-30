using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Mef.Host
{
    // Simple ALC. Taken from MSBuildLoadContext in dotnet/msbuild
    public sealed class IsolatedLoadContext : CustomLoadContextBase
    {
        private readonly string _directory;

        public IsolatedLoadContext(IImmutableSet<string> assembliesToUnify, string assemblyPath)
            : base(assembliesToUnify, $"Isolated plugin {assemblyPath}")
        {
            _directory = Directory.GetParent(assemblyPath)!.FullName;
        }

        protected override Assembly? InternalLoad(AssemblyName assemblyName)
        {
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
}
