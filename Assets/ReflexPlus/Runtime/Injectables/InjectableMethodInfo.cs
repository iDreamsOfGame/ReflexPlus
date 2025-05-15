using System.Reflection;

namespace ReflexPlus.Injectables
{
    public readonly struct InjectableMethodInfo
    {
        public InjectableMethodInfo(object[] keys, MethodInfo methodInfo, bool optional)
        {
            Keys = keys;
            MethodInfo = methodInfo;
            Parameters = methodInfo.GetParameters();
            Optional = optional;
        }

        public object[] Keys { get; }

        public MethodInfo MethodInfo { get; }

        public ParameterInfo[] Parameters { get; }

        public bool Optional { get; }
    }
}