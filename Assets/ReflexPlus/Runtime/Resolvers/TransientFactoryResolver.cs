﻿using System;
using ReflexPlus.Core;

namespace ReflexPlus.Resolvers
{
    internal sealed class TransientFactoryResolver : IResolver
    {
        private readonly Func<Container, object> factory;

        public Lifetime Lifetime => Lifetime.Transient;

        public Resolution Resolution => Resolution.Lazy;

        public TransientFactoryResolver(Func<Container, object> factory)
        {
            Diagnosis.RegisterCallSite(this);
            this.factory = factory;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);
            var instance = factory.Invoke(container);
            container.Disposables.TryAdd(instance);
            Diagnosis.RegisterInstance(this, instance);
            return instance;
        }

        public void Dispose()
        {
        }
    }
}