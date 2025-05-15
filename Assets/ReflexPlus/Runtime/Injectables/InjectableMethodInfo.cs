using System.Reflection;

namespace ReflexPlus.Injectables
{
    public readonly struct InjectableMethodInfo
    {
        public InjectableMethodInfo(object[] parameterKeys, MethodInfo methodInfo, bool optional)
        {
            ParameterKeys = parameterKeys;
            MethodInfo = methodInfo;
            Parameters = methodInfo.GetParameters();
            Optional = optional;
        }

        public object[] ParameterKeys { get; }

        public MethodInfo MethodInfo { get; }

        public ParameterInfo[] Parameters { get; }

        public bool Optional { get; }
    }
}