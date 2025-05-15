using System;
using System.Runtime.CompilerServices;
using ReflexPlus.Core;

namespace ReflexPlus.Resolvers
{
    internal sealed class ScopedTypeResolver : IResolver
    {
        private readonly Type concreteType;

        private readonly object key;

        private readonly ConditionalWeakTable<Container, object> instances = new();

        public Lifetime Lifetime => Lifetime.Scoped;

        public Resolution Resolution { get; }

        public ScopedTypeResolver(Type concreteType, object key, Resolution resolution)
        {
            Diagnosis.RegisterCallSite(this);
            this.concreteType = concreteType;
            this.key = key;
            Resolution = resolution;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);

            if (!instances.TryGetValue(container, out var instance))
            {
                instance = container.Construct(concreteType, key);
                instances.Add(container, instance);
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