using System;
using System.Runtime.CompilerServices;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.Resolvers
{
    internal sealed class ScopedTypeResolver : IResolver
    {
        private readonly Type _concreteType;
        private readonly object _key;
        private readonly ConditionalWeakTable<Container, object> _instances = new();
        public Lifetime Lifetime => Lifetime.Scoped;
        public Resolution Resolution { get; }

        public ScopedTypeResolver(Type concreteType, object key, Resolution resolution)
        {
            Diagnosis.RegisterCallSite(this);
            _concreteType = concreteType;
            _key = key;
            Resolution = resolution;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);

            if (!_instances.TryGetValue(container, out var instance))
            {
                instance = container.Construct(_concreteType, _key);
                _instances.Add(container, instance);
                container.Disposables.TryAdd(instance);
                Diagnosis.RegisterInstance(this, instance);
            }
            
            return instance;
        }

        public void Dispose()
        {
        }
    }
}