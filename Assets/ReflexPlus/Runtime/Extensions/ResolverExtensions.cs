using System.Runtime.CompilerServices;
using ReflexPlus.Resolvers;

namespace ReflexPlus.Extensions
{
    internal static class ResolverExtensions
    {
        private static readonly ConditionalWeakTable<IResolver, ResolverDebugProperties> Registry = new();

        public static ResolverDebugProperties GetDebugProperties(this IResolver resolver)
        {
            return Registry.GetOrCreateValue(resolver);
        }
    }
}