using System;
using JetBrains.Annotations;

namespace ReflexPlus.Attributes
{
    [MeansImplicitUse(ImplicitUseKindFlags.Assign)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method)]
    public class InjectAttribute : Attribute
    {
        public InjectAttribute()
        {
        }

        public InjectAttribute(bool optional)
        {
            Optional = optional;
        }

        public InjectAttribute(params object[] names)
        {
            Names = names;
        }

        public InjectAttribute(bool optional, params object[] names)
        {
            Optional = optional;
            Names = names;
        }

        public bool Optional { get; }

        public object[] Names { get; }

        public object Name
        {
            get
            {
                if (Names == null || Names.Length == 0)
                    return null;

                return Names[0];
            }
        }
    }
}