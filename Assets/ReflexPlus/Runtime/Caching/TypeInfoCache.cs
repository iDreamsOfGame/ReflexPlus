using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReflexPlus.Attributes;
using ReflexPlus.Injectables;

// ReSharper disable LoopCanBeConvertedToQuery

namespace ReflexPlus.Caching
{
    internal static class TypeInfoCache
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        private static readonly List<InjectableFieldInfo> Fields = new();

        private static readonly List<InjectablePropertyInfo> Properties = new();

        private static readonly List<InjectableMethodInfo> Methods = new();

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
            while (true)
            {
                var fields = type.GetFields(Flags)
                    .Where(f => f.IsDefined(typeof(InjectAttribute)));
                var injectableFields = new List<InjectableFieldInfo>();
                foreach (var field in fields)
                {
                    var injectAttribute = field.GetCustomAttribute<InjectAttribute>();
                    var injectableField = new InjectableFieldInfo(injectAttribute.Name, field, injectAttribute.Optional);
                    injectableFields.Add(injectableField);
                }

                var properties = type.GetProperties(Flags)
                    .Where(p => p.CanWrite && p.IsDefined(typeof(InjectAttribute)));
                var injectableProperties = new List<InjectablePropertyInfo>();
                foreach (var property in properties)
                {
                    var injectAttribute = property.GetCustomAttribute<InjectAttribute>();
                    var injectableProperty = new InjectablePropertyInfo(injectAttribute.Name, property, injectAttribute.Optional);
                    injectableProperties.Add(injectableProperty);
                }

                var methods = type.GetMethods(Flags)
                    .Where(m => m.IsDefined(typeof(InjectAttribute)));
                var injectableMethods = new List<InjectableMethodInfo>();
                foreach (var method in methods)
                {
                    var injectAttribute = method.GetCustomAttribute<InjectAttribute>();
                    var injectableMethod = new InjectableMethodInfo(injectAttribute.Names, method, injectAttribute.Optional);
                    injectableMethods.Add(injectableMethod);
                }

                Fields.AddRange(injectableFields);
                Properties.AddRange(injectableProperties);
                Methods.AddRange(injectableMethods);

                if (type.BaseType != null)
                {
                    type = type.BaseType;
                }
                else
                {
                    break;
                }
            }
        }
    }
}