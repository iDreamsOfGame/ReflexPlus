using System.Reflection;

namespace ReflexPlus.Injectables
{
    public readonly struct InjectableFieldInfo
    {
        public InjectableFieldInfo(object key, FieldInfo fieldInfo, bool optional)
        {
            Key = key;
            FieldInfo = fieldInfo;
            Optional = optional;
        }

        public object Key { get; }

        public FieldInfo FieldInfo { get; }

        public bool Optional { get; }
    }
}