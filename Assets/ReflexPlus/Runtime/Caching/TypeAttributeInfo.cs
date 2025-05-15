using System.Linq;
using System.Reflection;
using ReflexPlus.Injectables;

namespace ReflexPlus.Caching
{
    internal sealed class TypeAttributeInfo
    {
        public TypeAttributeInfo(InjectableFieldInfo[] injectableFields, InjectablePropertyInfo[] injectableProperties, MethodInfo[] injectableMethods)
        {
            InjectableFields = injectableFields;
            InjectableProperties = injectableProperties;
            InjectableMethods = injectableMethods.Select(mi => new InjectedMethodInfo(mi)).ToArray();
        }

        public InjectableFieldInfo[] InjectableFields { get; }

        public InjectablePropertyInfo[] InjectableProperties { get; }

        public InjectedMethodInfo[] InjectableMethods { get; }
    }
}