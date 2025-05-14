using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reflex.Attributes;
using Reflex.Injection;
using Reflex.Registration;

namespace Reflex.Caching
{
    internal static class TypeInfoCache
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        private static readonly List<InjectableFieldInfo> _fields = new();
        private static readonly List<PropertyInfo> _properties = new();
        private static readonly List<MethodInfo> _methods = new();

        private static readonly Dictionary<RegistrationId, TypeAttributeInfo> _dictionary = new();
        
        internal static TypeAttributeInfo Get(Type type, object key = null)
        {
            var registrationId = new RegistrationId(type, key);
            if (!_dictionary.TryGetValue(registrationId, out var info))
            {
                _fields.Clear();
                _properties.Clear();
                _methods.Clear();
                Generate(type, key);
                info = new TypeAttributeInfo(_fields.ToArray(), _properties.ToArray(), _methods.ToArray());
                _dictionary.Add(registrationId, info);
            }
    
            return info;
        }
        
        private static void Generate(Type type, object key)
        {
            var fields = type
                .GetFields(Flags)
                .Where(f => f.IsDefined(typeof(InjectAttribute)));
            var injectableFields = new List<InjectableFieldInfo>();
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<InjectAttribute>();
                var registrationId = new RegistrationId(field.FieldType, key);
                var injectableField = new InjectableFieldInfo(registrationId, field, attribute.Optional);
                injectableFields.Add(injectableField);
            }

            var properties = type
                .GetProperties(Flags)
                .Where(p => p.CanWrite && p.IsDefined(typeof(InjectAttribute)));

            var methods = type
                .GetMethods(Flags)
                .Where(m => m.IsDefined(typeof(InjectAttribute)));
            
            _fields.AddRange(injectableFields);
            _properties.AddRange(properties);
            _methods.AddRange(methods);

            if (type.BaseType != null)
            {
                Generate(type.BaseType, key);
            }
        }
    }
}