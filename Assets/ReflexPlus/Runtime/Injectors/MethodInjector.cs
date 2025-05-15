using System;
using ReflexPlus.Caching;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

namespace ReflexPlus.Injectors
{
    internal static class MethodInjector
    {
        [ThreadStatic]
        private static SizeSpecificArrayPool<object> arrayPool;

        private static SizeSpecificArrayPool<object> ArrayPool => arrayPool ??= new SizeSpecificArrayPool<object>(maxLength: 16);

        internal static void Inject(InjectedMethodInfo method, object instance, Container container)
        {
            var methodParameters = method.Parameters;
            var methodParametersLength = methodParameters.Length;
            var arguments = ArrayPool.Rent(methodParametersLength);

            try
            {
                for (var i = 0; i < methodParametersLength; i++)
                {
                    arguments[i] = container.Resolve(methodParameters[i].ParameterType);
                }

                method.MethodInfo.Invoke(instance, arguments);
            }
            catch (Exception e)
            {
                throw new MethodInjectorException(instance, method.MethodInfo, e);
            }
            finally
            {
                ArrayPool.Return(arguments);
            }
        }
    }
}