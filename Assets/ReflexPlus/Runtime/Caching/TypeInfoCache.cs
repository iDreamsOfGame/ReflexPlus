using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReflexPlus.Attributes;
using ReflexPlus.Injectables;

namespace ReflexPlus.Caching
{
    internal static class TypeInfoCache
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        private static readonly List<InjectableFieldInfo> Fields = new();

        private static readonly List<PropertyInfo> Properties = new();

        private static readonly List<MethodInfo> Methods = new();

        private static readonly Dictionary<RegistrationId, TypeAttributeInfo> Dictionary = new();

        internal static TypeAttributeInfo Get(Type type, object key = null)
        {
            var registrationId = new RegistrationId(type, key);
            if (!Dictionary.TryGetValue(registrationId, out var info))
            {
                Fields.Clear();
                Properties.Clear();
                Methods.Clear();
                Generate(type);
                info = new TypeAttributeInfo(Fields.ToArray(), Properties.ToArray(), Methods.ToArray());
                Dictionary.Add(registrationId, info);
            }

            return info;
        }

        private static void Generate(Type type)
        {
            var fields = type
                .GetFields(Flags)
                .Where(f => f.IsDefined(typeof(InjectAttribute)));
            var injectableFields = new List<InjectableFieldInfo>();
            foreach (var field in fields)
            {
                var attribute = field.GetCustomAttribute<InjectAttribute>();
                var registrationId = new RegistrationId(field.FieldType, attribute.Name);
                var injectableField = new InjectableFieldInfo(registrationId, field, attribute.Optional);
                injectableFields.Add(injectableField);
            }

            var properties = type
                .GetProperties(Flags)
                .Where(p => p.CanWrite && p.IsDefined(typeof(InjectAttribute)));

            var methods = type
                .GetMethods(Flags)
                .Where(m => m.IsDefined(typeof(InjectAttribute)));

            Fields.AddRange(injectableFields);
            Properties.AddRange(properties);
            Methods.AddRange(methods);

            if (type.BaseType != null)
            {
                Generate(type.BaseType);
            }
        }
    }
}