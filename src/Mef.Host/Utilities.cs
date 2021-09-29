using System.ComponentModel.Composition;
using System.Reflection;

namespace Mef.Host
{
    public static class Utilities
    {
        public static bool IsExport(ICustomAttributeProvider attributeProvider)
        {
            return attributeProvider.IsAttributeDefined<ExportAttribute>(false);
        }
        public static AssemblyName GetAssemblyNameWithCodebasePath(string path)
        {
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(path);

            // .NET Framework sets this, but .NET Core does not.
            // .NET Core also *ignores* this even if set, by default. But a custom AssemblyLoadContext resolver could honor it if we preserve it.
            assemblyName.CodeBase = path;

            return assemblyName;
        }
    }
}
