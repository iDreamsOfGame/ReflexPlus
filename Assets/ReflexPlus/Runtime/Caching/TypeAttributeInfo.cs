using ReflexPlus.Injectables;

namespace ReflexPlus.Caching
{
    internal sealed class TypeAttributeInfo
    {
        public TypeAttributeInfo(InjectableFieldInfo[] injectableFields, InjectablePropertyInfo[] injectableProperties, InjectableMethodInfo[] injectableMethods)
        {
            InjectableFields = injectableFields;
            InjectableProperties = injectableProperties;
            InjectableMethods = injectableMethods;
        }

        public InjectableFieldInfo[] InjectableFields { get; }

        public InjectablePropertyInfo[] InjectableProperties { get; }

        public InjectableMethodInfo[] InjectableMethods { get; }
    }
}