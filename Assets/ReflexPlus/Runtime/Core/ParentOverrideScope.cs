using System;
using ReflexPlus.Injectors;

namespace ReflexPlus.Core
{
    public class ParentOverrideScope : IDisposable
    {
        private readonly Container parentOverride;

        public ParentOverrideScope(Container parentOverride)
        {
            this.parentOverride = parentOverride;
            UnityInjector.ContainerParentOverride.Push(this.parentOverride);
        }

        public void Dispose()
        {
            if (UnityInjector.ContainerParentOverride.TryPop(out var popped) && popped == parentOverride)
            {
                // All good, we popped the correct parent override
            }
            else
            {
                throw new InvalidOperationException("ParentOverrideScope was not disposed in the correct order.");
            }
        }
    }
}