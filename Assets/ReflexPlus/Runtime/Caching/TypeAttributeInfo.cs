using System.Linq;
using System.Reflection;
using Reflex.Core;
using Reflex.Injection;

namespace Reflex.Caching
{
    internal sealed class TypeAttributeInfo
    {
        public TypeAttributeInfo(InjectableFieldInfo[] injectableFields, PropertyInfo[] injectableProperties, MethodInfo[] injectableMethods)
        {
            InjectableFields = injectableFields;
            InjectableProperties = injectableProperties;
            InjectableMethods = injectableMethods.Select(mi => new InjectedMethodInfo(mi)).ToArray();
        }

        public InjectableFieldInfo[] InjectableFields { get; }

        public PropertyInfo[] InjectableProperties { get; }

        public InjectedMethodInfo[] InjectableMethods { get; }

        public void InjectInfoFields(object obj, object key, Container container)
        {
            var fieldCount = InjectableFields.Length;
            for (var i = 0; i < fieldCount; i++)
            {
                InjectableFields[i].InjectInto(obj, key, container);
            }
        }
    }
}