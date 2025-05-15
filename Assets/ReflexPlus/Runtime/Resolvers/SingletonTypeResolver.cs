using System;
using ReflexPlus.Core;

namespace ReflexPlus.Resolvers
{
    internal sealed class SingletonTypeResolver : IResolver
    {
        private object instance;

        private readonly Type concreteType;

        private readonly object key;

        private readonly DisposableCollection disposables = new();

        public Lifetime Lifetime => Lifetime.Singleton;

        public Resolution Resolution { get; }

        public SingletonTypeResolver(Type concreteType, object key, Resolution resolution)
        {
            Diagnosis.RegisterCallSite(this);
            this.concreteType = concreteType;
            this.key = key;
            Resolution = resolution;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);

            if (instance == null)
            {
                instance = container.Construct(concreteType, key);
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