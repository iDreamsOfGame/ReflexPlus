using System;
using ReflexPlus.Core;

namespace ReflexPlus.Resolvers
{
    internal sealed class SingletonFactoryResolver : IResolver
    {
        private object instance;

        private readonly Func<Container, object> factory;

        private readonly DisposableCollection disposables = new();

        public Lifetime Lifetime => Lifetime.Singleton;

        public Resolution Resolution { get; }

        public SingletonFactoryResolver(Func<Container, object> factory, Resolution resolution)
        {
            Diagnosis.RegisterCallSite(this);
            this.factory = factory;
            Resolution = resolution;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);

            if (instance == null)
            {
                instance = factory.Invoke(container);
                disposables.TryAdd(instance);
                Diagnosis.RegisterInstance(this, instance);
            }

            return instance;
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }
}