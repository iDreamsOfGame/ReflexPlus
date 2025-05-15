using System;

namespace ReflexPlus.Caching
{
    internal sealed class TypeConstructionInfo
    {
        public TypeConstructionInfo(ObjectActivator objectActivator,
            Type[] constructorParameters,
            bool optional = false,
            object[] parameterKeys = null)
        {
            ObjectActivator = objectActivator;
            ConstructorParameters = constructorParameters;
            Optional = optional;
            ParameterKeys = parameterKeys;
        }

        public ObjectActivator ObjectActivator { get; }

        public Type[] ConstructorParameters { get; }

        public bool Optional { get; }

        public object[] ParameterKeys { get; }
    }
}