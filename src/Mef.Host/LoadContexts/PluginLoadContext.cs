using System;
using System.Collections.Immutable;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Mef.Host
{
    // From MSDN documentation on ALCs
    // https://docs.microsoft.com/en-us/dotnet/core/tutorials/creating-app-with-plugin-support
    public class PluginLoadContext : CustomLoadContextBase
    {
        private readonly AssemblyDependencyResolver _resolver;

        public PluginLoadContext(IImmutableSet<string> assembliesToUnify, string pluginPath)
            : base(assembliesToUnify, pluginPath)
        {
            if (!Path.HasExtension(pluginPath))
            {
                throw new ArgumentException("Plugin path must be path to component, not directory", nameof(pluginPath));
            }
            _resolver = new AssemblyDependencyResolver(pluginPath);
        }

        protected sealed override Assembly? InternalLoad(AssemblyName assemblyName)
        {
            var assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (assemblyPath != null)
            {
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }
}
