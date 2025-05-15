using System;
using ReflexPlus.Caching;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

namespace ReflexPlus.Injectors
{
    public static class ConstructorInjector
    {
        [ThreadStatic]
        private static SizeSpecificArrayPool<object> arrayPool;

        internal static SizeSpecificArrayPool<object> ArrayPool => arrayPool ??= new SizeSpecificArrayPool<object>(maxLength: 16);

        public static object Construct(Type concrete, Container container)
        {
            var info = TypeConstructionInfoCache.Get(concrete);
            var optional = info.Optional;
            var parameterKeys = info.ParameterKeys ?? Array.Empty<object>();
            var parameterKeysLength = parameterKeys.Length;
            var constructorParameters = info.ConstructorParameters;
            var constructorParametersLength = info.ConstructorParameters.Length;
            var arguments = ArrayPool.Rent(constructorParametersLength);

            try
            {
                for (var i = 0; i < constructorParametersLength; i++)
                {
                    var parameterKey = i < parameterKeysLength ? parameterKeys[i] : null;
                    arguments[i] = container.Resolve(constructorParameters[i], optional, parameterKey);
                }

                return info.ObjectActivator.Invoke(arguments);
            }
            catch (Exception exception)
            {
                if (!info.Optional) 
                    throw new ConstructorInjectorException(concrete, exception, constructorParameters);

                return null;
            }
            finally
            {
                ArrayPool.Return(arguments);
            }
        }

        public static object Construct(Type concrete, object[] arguments)
        {
            var info = TypeConstructionInfoCache.Get(concrete);

            try
            {
                return info.ObjectActivator.Invoke(arguments);
            }
            catch (Exception exception)
            {
                if (!info.Optional)
                    throw new ConstructorInjectorException(concrete, exception, info.ConstructorParameters);

                return null;
            }
        }
    }
}