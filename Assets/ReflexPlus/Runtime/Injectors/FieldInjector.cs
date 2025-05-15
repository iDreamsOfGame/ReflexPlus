using System;
using System.Reflection;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

namespace ReflexPlus.Injectors
{
    internal static class FieldInjector
    {
        internal static void Inject(FieldInfo field, bool optional, object key, object instance, Container container)
        {
            try
            {
                field.SetValue(instance, container.Resolve(field.FieldType, optional, key));
            }
            catch (Exception e)
            {
                if (!optional)
                    throw new FieldInjectorException(e);
            }
        }
    }
}