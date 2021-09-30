using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.Loader;

namespace Mef.Host
{
    /// <summary>
    /// Base class that all custom AssemblyLoadContexts should derive from
    /// to help follow needed constructor arguments
    /// </summary>
    public abstract class CustomLoadContextBase : AssemblyLoadContext
    {
        private readonly IImmutableSet<string> _assembliesToUnify;

        public CustomLoadContextBase(IImmutableSet<string> assembliesToUnify, string? name = default)
            : base(name)
        {
            _assembliesToUnify = assembliesToUnify;
        }

        protected sealed override Assembly? Load(AssemblyName assemblyName)
        {
            if (_assembliesToUnify.Contains(assemblyName.Name!))
            {
                return null;
            }

            return InternalLoad(assemblyName);
        }

        protected abstract Assembly? InternalLoad(AssemblyName assemblyName);
    }
}
