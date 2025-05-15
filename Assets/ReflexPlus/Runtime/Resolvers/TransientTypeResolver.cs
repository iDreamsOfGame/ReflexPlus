using System;
using ReflexPlus.Core;

namespace ReflexPlus.Resolvers
{
    internal sealed class TransientTypeResolver : IResolver
    {
        private readonly Type concreteType;

        private readonly object key;

        public Lifetime Lifetime => Lifetime.Transient;

        public Resolution Resolution => Resolution.Lazy;

        public TransientTypeResolver(Type concreteType, object key)
        {
            Diagnosis.RegisterCallSite(this);
            this.concreteType = concreteType;
            this.key = key;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);
            var instance = container.Construct(concreteType, key);
            container.Disposables.TryAdd(instance);
            Diagnosis.RegisterInstance(this, instance);
            return instance;
        }

        public void Dispose()
        {
        }
    }
}