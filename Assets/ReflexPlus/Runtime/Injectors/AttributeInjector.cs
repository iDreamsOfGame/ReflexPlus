using ReflexPlus.Caching;
using ReflexPlus.Core;
using ReflexPlus.Injectables;

namespace ReflexPlus.Injectors
{
    public static class AttributeInjector
    {
        public static void InjectInto(object obj, Container container)
        {
            InjectInto(obj, null, container);
        }
        
        public static void InjectInto(object obj, object key, Container container)
        {
            var info = TypeInfoCache.Get(obj.GetType(), key);
            InjectFields(info.InjectableFields, obj, container);
            InjectProperties(info.InjectableProperties, obj, container);
            InjectMethods(info.InjectableMethods, obj, container);
        }

        private static void InjectFields(InjectableFieldInfo[] injectableFields, object obj, Container container)
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
        
        private static void InjectProperties(InjectablePropertyInfo[] injectableProperties, object obj, Container container)
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

        private static void InjectMethods(InjectableMethodInfo[] injectableMethods, object obj, Container container)
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