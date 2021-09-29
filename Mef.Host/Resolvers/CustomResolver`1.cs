
using Microsoft.VisualStudio.Composition;

namespace Mef.Host
{
    public class CustomResolver<T> : Resolver where T : IAssemblyLoader, new()
    {
        public CustomResolver() : base(new T()) { }
    }
}
