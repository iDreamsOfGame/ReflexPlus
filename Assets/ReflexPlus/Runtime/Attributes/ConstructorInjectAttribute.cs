using System;
using JetBrains.Annotations;

namespace ReflexPlus.Attributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    [AttributeUsage(AttributeTargets.Constructor)]
    public class ConstructorInjectAttribute : Attribute
    {
        public ConstructorInjectAttribute()
        {
        }
        
        public ConstructorInjectAttribute(bool optional)
        {
            Optional = optional;
        }
        
        public ConstructorInjectAttribute(params object[] parameterNames)
        {
            ParameterNames = parameterNames;
        }

        public ConstructorInjectAttribute(bool optional, params object[] parameterNames)
        {
            Optional = optional;
            ParameterNames = parameterNames;
        }
        
        public bool Optional { get; }
        
        public object[] ParameterNames { get; }
    }
}