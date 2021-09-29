using System.Reflection;

namespace Mef.Host
{
    public static class AttributeServices
    {
        public static bool IsAttributeDefined<T>(this ICustomAttributeProvider attributeProvider, bool inherit) where T : class
        {
            return attributeProvider.IsDefined(typeof(T), inherit);
        }
    }
}
