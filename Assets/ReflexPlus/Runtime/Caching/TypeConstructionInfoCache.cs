using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ReflexPlus.Extensions;
using ReflexPlus.Attributes;
using ReflexPlus.Reflectors;

namespace ReflexPlus.Caching
{
    internal static class TypeConstructionInfoCache
    {
        private static readonly Dictionary<IntPtr, TypeConstructionInfo> Dictionary = new();

        internal static TypeConstructionInfo Get(Type type)
        {
            var key = type.TypeHandle.Value;
            if (!Dictionary.TryGetValue(key, out var info))
            {
                info = Generate(type);
                Dictionary.Add(key, info);
            }

            return info;
        }

        private static TypeConstructionInfo Generate(Type type)
        {
            if (type.TryGetConstructors(out var constructors))
            {
                var constructor = constructors.FirstOrDefault(c => Attribute.IsDefined(c, typeof(ConstructorInjectAttribute))); // Try to get a constructor that defines ReflexConstructor
                if (constructor == null)
                    constructor = constructors.MaxBy(ctor => ctor.GetParameters().Length); // Gets the constructor with most arguments
                
                var injectAttribute = constructor.GetCustomAttribute<ConstructorInjectAttribute>();
                var parameters = constructor.GetParameters().Select(p => p.ParameterType).ToArray();
                var optional = injectAttribute?.Optional ?? false;
                var parameterKeys = injectAttribute?.ParameterNames;
                return new TypeConstructionInfo(ActivatorFactoryManager.Factory.GenerateActivator(type, constructor, parameters), parameters, optional, parameterKeys);
            }
            
            return new TypeConstructionInfo(ActivatorFactoryManager.Factory.GenerateDefaultActivator(type), Type.EmptyTypes);
        }
    }
}