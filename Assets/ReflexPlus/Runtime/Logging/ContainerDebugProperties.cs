using System.Collections.Generic;

namespace ReflexPlus.Logging
{
    public sealed class ContainerDebugProperties
    {
        public List<CallSite> BuildCallsite { get; } = new();
    }
}