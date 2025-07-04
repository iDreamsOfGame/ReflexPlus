﻿using System;
using System.Runtime.CompilerServices;
using ReflexPlus.Core;

namespace ReflexPlus.Resolvers
{
    internal sealed class ScopedFactoryResolver : IResolver
    {
        private readonly Func<Container, object> _factory;

        private readonly ConditionalWeakTable<Container, object> _instances = new();

        public Lifetime Lifetime => Lifetime.Scoped;

        public Resolution Resolution { get; }

        public ScopedFactoryResolver(Func<Container, object> factory, Resolution resolution)
        {
            Diagnosis.RegisterCallSite(this);
            _factory = factory;
            Resolution = resolution;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);

            if (!_instances.TryGetValue(container, out var instance))
            {
                instance = _factory.Invoke(container);
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