using System;
using Reflex.Core;
using Reflex.Enums;
using Reflex.Generics;

namespace Reflex.Resolvers
{
    internal sealed class SingletonTypeResolver : IResolver
    {
        private object _instance;
        private readonly Type _concreteType;
        private readonly object _key;
        private readonly DisposableCollection _disposables = new();
        public Lifetime Lifetime => Lifetime.Singleton;
        public Resolution Resolution { get; }

        public SingletonTypeResolver(Type concreteType, object key, Resolution resolution)
        {
            Diagnosis.RegisterCallSite(this);
            _concreteType = concreteType;
            _key = key;
            Resolution = resolution;
        }

        public object Resolve(Container container)
        {
            Diagnosis.IncrementResolutions(this);

            if (_instance == null)
            {
                _instance = container.Construct(_concreteType, _key);
                _disposables.TryAdd(_instance);
                Diagnosis.RegisterInstance(this, _instance);
            }

            return _instance;
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}