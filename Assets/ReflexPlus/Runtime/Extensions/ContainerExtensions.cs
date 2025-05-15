using System.Runtime.CompilerServices;
using ReflexPlus.Core;
using ReflexPlus.Logging;

namespace ReflexPlus.Extensions
{
    public static class ContainerExtensions
    {
        private static readonly ConditionalWeakTable<Container, ContainerDebugProperties> ContainerDebugProperties = new();

        internal static ContainerDebugProperties GetDebugProperties(this Container container)
        {
            return ContainerDebugProperties.GetOrCreateValue(container);
        }
    }
}