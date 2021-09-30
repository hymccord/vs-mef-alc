using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;

using Microsoft.VisualStudio.Composition;

namespace Mef.Host
{
    public abstract class BaseAssemblyLoader<T> : IAssemblyLoader where T : CustomLoadContextBase
    {
        /// <summary>
        /// A cache of assembly names to loaded assemblies.
        /// </summary>
        private readonly Dictionary<AssemblyName, Assembly> _loadedAssemblies = new(ByValueEquality.AssemblyName);
        private readonly ConcurrentDictionary<AssemblyName, AssemblyLoadContext> _loadedContexts = new(ByValueEquality.AssemblyName);

        /// <inheritdoc />
        public Assembly LoadAssembly(AssemblyName assemblyName)
        {
            Assembly? assembly;
            lock (_loadedAssemblies)
            {
                _loadedAssemblies.TryGetValue(assemblyName, out assembly);
            }

            if (assemblyName.CodeBase is null)
            {
                throw new ArgumentException("Codebase cannot be null", nameof(assemblyName));
            }

            if (assembly == null)
            {
                AssemblyLoadContext loadContext;
                lock (_loadedContexts)
                {
                    // You get an ALC! You get an ALC! EVERYBODY GETS ALCs!
                    // The custom ALC can handle if the assembly will be loaded into the Default context if needed (like IsolatedLoadContext)
                    loadContext = _loadedContexts.GetOrAdd(assemblyName, (an) =>
                    {
                        return (AssemblyLoadContext)Activator.CreateInstance(typeof(T), new object[] { AssemblyUnification.WellKnownAssemblyNames, assemblyName.CodeBase })!;
                    });
                }

                assembly = loadContext.LoadFromAssemblyPath(assemblyName.CodeBase);


                lock (_loadedAssemblies)
                {
                    _loadedAssemblies[assemblyName] = assembly;
                }
            }

            return assembly;
        }

        /// <inheritdoc />
        /// <remarks>This one is never used by VS-MEF currently.</remarks>
        public Assembly LoadAssembly(string assemblyFullName, string? codeBasePath)
        {
            throw new NotImplementedException();
        }
    }
}
