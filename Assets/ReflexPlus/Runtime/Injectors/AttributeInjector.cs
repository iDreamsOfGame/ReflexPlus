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

            var methods = info.InjectableMethods;
            var methodCount = methods.Length;
            for (var i = 0; i < methodCount; i++)
            {
                MethodInjector.Inject(methods[i], obj, container);
            }
        }

        private static void InjectIntoFields(InjectableFieldInfo[] injectableFields, object obj, Container container)
        {
            var fieldCount = injectableFields.Length;
            for (var i = 0; i < fieldCount; i++)
            {
                var injectableFieldInfo = injectableFields[i];
                var registrationId = injectableFieldInfo.RegistrationId;
                var fieldInfo = injectableFieldInfo.FieldInfo;
                var optional = injectableFieldInfo.Optional;
                FieldInjector.Inject(fieldInfo, optional, registrationId.Key, obj, container);
            }
        }
        
        private static void InjectIntoProperties(InjectablePropertyInfo[] injectableProperties, object obj, Container container)
        {
            var propertyCount = injectableProperties.Length;
            for (var i = 0; i < propertyCount; i++)
            {
                var injectablePropertyInfo = injectableProperties[i];
                var registrationId = injectablePropertyInfo.RegistrationId;
                var propertyInfo = injectablePropertyInfo.PropertyInfo;
                var optional = injectablePropertyInfo.Optional;
                PropertyInjector.Inject(propertyInfo, optional, registrationId.Key, obj, container);
            }
        }
    }
}