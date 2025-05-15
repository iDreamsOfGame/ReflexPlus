using System;
using System.Reflection;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

namespace ReflexPlus.Injectors
{
    internal static class PropertyInjector
    {
        internal static void Inject(PropertyInfo property, object instance, Container container)
        {
            try
            {
                property.SetValue(instance, container.Resolve(property.PropertyType));
            }
            catch (Exception e)
            {
                throw new PropertyInjectorException(e);
            }
        }
    }
}