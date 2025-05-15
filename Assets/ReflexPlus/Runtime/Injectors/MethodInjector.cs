using System;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;
using ReflexPlus.Injectables;

namespace ReflexPlus.Injectors
{
    internal static class MethodInjector
    {
        [ThreadStatic]
        private static SizeSpecificArrayPool<object> arrayPool;

        private static SizeSpecificArrayPool<object> ArrayPool => arrayPool ??= new SizeSpecificArrayPool<object>(maxLength: 16);

        internal static void Inject(InjectableMethodInfo method, object instance, Container container)
        {
            var optional = method.Optional;
            var keys = method.Keys ?? Array.Empty<object>();
            var keysLength = keys.Length;
            var methodParameters = method.Parameters;
            var methodParametersLength = methodParameters.Length;
            var arguments = ArrayPool.Rent(methodParametersLength);

            try
            {
                for (var i = 0; i < methodParametersLength; i++)
                {
                    var key = i < keysLength ? keys[i] : null;
                    arguments[i] = container.Resolve(methodParameters[i].ParameterType, optional, key);
                }

                method.MethodInfo.Invoke(instance, arguments);
            }
            catch (Exception e)
            {
                if (!optional) 
                    throw new MethodInjectorException(instance, method.MethodInfo, e);
            }
            finally
            {
                ArrayPool.Return(arguments);
            }
        }
    }
}