using System.Reflection;

namespace ReflexPlus.Injectables
{
    public readonly struct InjectablePropertyInfo
    {
        public InjectablePropertyInfo(object key, PropertyInfo propertyInfo, bool optional)
        {
            Key = key;
            PropertyInfo = propertyInfo;
            Optional = optional;
        }

        public object Key { get; }

        public PropertyInfo PropertyInfo { get; }

        public bool Optional { get; }
    }
}