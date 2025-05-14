using System;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class TransientTypeResolver : IResolver
    {
        private readonly Type _concreteType;
        private readonly object _key;
        public Lifetime Lifetime => Lifetime.Transient;
        public Resolution Resolution => Resolution.Lazy;

        public TransientTypeResolver(Type concreteType, object key)
        {
            Diagnosis.RegisterCallSite(this);
            _concreteType = concreteType;
            _key = key;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);
            var instance = container.Construct(_concreteType, _key);
            container.Disposables.TryAdd(instance);
            Diagnosis.RegisterInstance(this, instance);
            return instance;
        }

        public void Dispose()
        {
        }
    }
}