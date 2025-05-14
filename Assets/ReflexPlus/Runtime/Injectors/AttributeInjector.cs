using Reflex.Caching;
using Reflex.Core;

namespace Reflex.Injectors
{
    public static class AttributeInjector
    {
        public static void Inject(object obj, object key, Container container)
        {
            var info = TypeInfoCache.Get(obj.GetType(), key);
            info.InjectInfoFields(obj, key, container);

            var properties = info.InjectableProperties;
            var propertyCount = properties.Length;
            for (var i = 0; i < propertyCount; i++)
            {
                PropertyInjector.Inject(properties[i], obj, container);
            }

            var methods = info.InjectableMethods;
            var methodCount = methods.Length;
            for (var i = 0; i < methodCount; i++)
            {
                MethodInjector.Inject(methods[i], obj, container);
            }
        }
    }
}