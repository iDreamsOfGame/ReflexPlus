using System;
using System.Reflection;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

namespace ReflexPlus.Injectors
{
    internal static class PropertyInjector
    {
        internal static void Inject(PropertyInfo property, bool optional, object key, object instance, Container container)
        {
            try
            {
                property.SetValue(instance, container.Resolve(property.PropertyType, optional, key));
            }
            catch (Exception e)
            {
                if (!optional)
                    throw new PropertyInjectorException(e);
            }
        }
    }
}