using System;
using System.Reflection;

namespace ReflexPlus.Reflectors
{
    internal interface IActivatorFactory
    {
        ObjectActivator GenerateActivator(Type type, ConstructorInfo constructor, Type[] parameters);

        ObjectActivator GenerateDefaultActivator(Type type);
    }
}