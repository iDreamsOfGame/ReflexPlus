using System;
using ReflexPlus.Utilities;

namespace ReflexPlus.Reflectors
{
    internal static class ActivatorFactoryManager
    {
        internal static readonly IActivatorFactory Factory;

        static ActivatorFactoryManager()
        {
            Factory = GetFactory();
        }

        private static IActivatorFactory GetFactory()
        {
            return ScriptingBackend.Current switch
            {
                ScriptingBackend.Backend.Mono => new MonoActivatorFactory(),
                ScriptingBackend.Backend.IL2CPP => new IL2CPPActivatorFactory(),
                ScriptingBackend.Backend.Undefined => throw new Exception("UndefinedRuntimeScriptingBackend"),
                _ => throw new Exception($"UnhandledRuntimeScriptingBackend {ScriptingBackend.Current}")
            };
        }
    }
}