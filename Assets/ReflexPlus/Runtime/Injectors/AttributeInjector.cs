using ReflexPlus.Caching;
using ReflexPlus.Core;
using ReflexPlus.Injectables;

namespace ReflexPlus.Injectors
{
    public static class AttributeInjector
    {
        public static void Inject(object obj, object key, Container container)
        {
            var info = TypeInfoCache.Get(obj.GetType(), key);
            InjectIntoFields(info.InjectableFields, obj, container);
            InjectIntoProperties(info.InjectableProperties, obj, container);
            InjectIntoMethods(info.InjectableMethods, obj, container);
        }

        private static void InjectIntoFields(InjectableFieldInfo[] injectableFields, object obj, Container container)
        {
            var fieldCount = injectableFields.Length;
            for (var i = 0; i < fieldCount; i++)
            {
                var injectableField = injectableFields[i];
                var key = injectableField.Key;
                var fieldInfo = injectableField.FieldInfo;
                var optional = injectableField.Optional;
                FieldInjector.Inject(fieldInfo, optional, key, obj, container);
            }
        }
        
        private static void InjectIntoProperties(InjectablePropertyInfo[] injectableProperties, object obj, Container container)
        {
            var propertyCount = injectableProperties.Length;
            for (var i = 0; i < propertyCount; i++)
            {
                var injectableProperty = injectableProperties[i];
                var key = injectableProperty.Key;
                var propertyInfo = injectableProperty.PropertyInfo;
                var optional = injectableProperty.Optional;
                PropertyInjector.Inject(propertyInfo, optional, key, obj, container);
            }
        }

        private static void InjectIntoMethods(InjectableMethodInfo[] injectableMethods, object obj, Container container)
        {
            var methodCount = injectableMethods.Length;
            for (var i = 0; i < methodCount; i++)
            {
                var injectableMethod = injectableMethods[i];
                MethodInjector.Inject(injectableMethod, obj, container);
            }
        }
    }
}