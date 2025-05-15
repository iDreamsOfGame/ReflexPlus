using System;
using JetBrains.Annotations;

namespace ReflexPlus.Attributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.InstantiatedNoFixedConstructorSignature)]
    [AttributeUsage(AttributeTargets.Constructor)]
    public class ReflexPlusConstructorAttribute : Attribute
    {
    }
}