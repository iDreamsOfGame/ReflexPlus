using System;

namespace ReflexPlus.Caching
{
    internal sealed class TypeConstructionInfo
    {
        public readonly ObjectActivator ObjectActivator;

        public readonly Type[] ConstructorParameters;

        public TypeConstructionInfo(ObjectActivator objectActivator, Type[] constructorParameters)
        {
            ObjectActivator = objectActivator;
            ConstructorParameters = constructorParameters;
        }
    }
}