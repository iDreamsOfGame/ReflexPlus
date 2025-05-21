using System;
using System.Collections.Generic;
using System.Linq;
using ReflexPlus.Injectors;
using ReflexPlus.Resolvers;
using UnityEngine.Assertions;

// ReSharper disable MergeIntoPattern

namespace ReflexPlus.Core
{
    public sealed class ContainerBuilder
    {
        public string Name { get; private set; }

        public Container Parent { get; private set; }

        public List<Binding> Bindings { get; } = new();

        public event Action<Container> OnContainerBuilt;

        public Container Build()
        {
            var disposables = new DisposableCollection();
            var resolversByContract = new Dictionary<RegistrationId, List<IResolver>>();

            // Extra installers
            UnityInjector.ExtraInstallers?.Invoke(this);

            // Parent override
            if (UnityInjector.ContainerParentOverride.TryPeek(out var parentOverride))
            {
                Parent = parentOverride;
            }

            // Inherited resolvers
            if (Parent != null)
            {
                foreach (var (registrationId, resolvers) in Parent.ResolversByContract)
                {
                    resolversByContract[registrationId] = new List<IResolver>(resolvers);
                }
            }

            // Owned resolvers
            foreach (var binding in Bindings)
            {
                disposables.Add(binding.Resolver);

                foreach (var contract in binding.Contracts)
                {
                    var registrationId = new RegistrationId(contract, binding.Key);
                    if (!resolversByContract.TryGetValue(registrationId, out var resolvers))
                    {
                        resolvers = new List<IResolver>();
                        resolversByContract.Add(registrationId, resolvers);
                    }

                    resolvers.Add(binding.Resolver);
                }
            }

            var container = new Container(Name, Parent, resolversByContract, disposables);

            // Eagerly resolve inherited Scoped + Eager bindings
            if (Parent != null)
            {
                var inheritedEagerResolvers = Parent.ResolversByContract
                    .SelectMany(kvp => kvp.Value)
                    .ToHashSet()
                    .Where(r => r.Lifetime == Lifetime.Scoped && r.Resolution == Resolution.Eager);

                foreach (var resolver in inheritedEagerResolvers)
                {
                    resolver.Resolve(container);
                }
            }

            // Eagerly resolve self Singleton/Scoped + Eager bindings
            if (Bindings != null)
            {
                var selfEagerResolvers = Bindings
                    .Select(b => b.Resolver)
                    .Where(r => r.Resolution == Resolution.Eager && r.Lifetime is Lifetime.Singleton or Lifetime.Scoped);

                foreach (var resolver in selfEagerResolvers)
                {
                    resolver.Resolve(container);
                }
            }

            Bindings.Clear();
            OnContainerBuilt?.Invoke(container);
            return container;
        }

        public ContainerBuilder SetName(string name)
        {
            Name = name;
            return this;
        }

        public ContainerBuilder SetParent(Container parent)
        {
            Parent = parent;
            return this;
        }

        public bool HasBinding(Type contract)
        {
            return Bindings.Any(binding => binding.Contracts.Contains(contract));
        }

        public ContainerBuilder RegisterType<T>(object key, Lifetime lifetime = Lifetime.Singleton, Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(typeof(T), key, lifetime, resolution);
        }

        public ContainerBuilder RegisterType<T>(Lifetime lifetime = Lifetime.Singleton, Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(typeof(T), lifetime, resolution);
        }

        public ContainerBuilder RegisterType<T>(Type[] contracts,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType<T>(contracts, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterType<T>(Type[] contracts,
            object key,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(typeof(T), contracts, key, lifetime, resolution);
        }

        public ContainerBuilder RegisterType(Type type,
            object key,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(type, new[] { type }, key, lifetime, resolution);
        }

        public ContainerBuilder RegisterType(Type type, Lifetime lifetime = Lifetime.Singleton, Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(type, new[] { type }, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterType(Type type,
            Type[] contracts,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(type, contracts, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterType(Type type,
            Type[] contracts,
            object key,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            Assert.IsNotNull(type);
            Assert.IsTrue(contracts != null && contracts.Length > 0);
            Assert.IsFalse(lifetime == Lifetime.Transient && resolution == Resolution.Eager, "Type registration Lifetime.Transient + Resolution.Eager not allowed");

            IResolver resolver = lifetime switch
            {
                Lifetime.Singleton => new SingletonTypeResolver(type, key, resolution),
                Lifetime.Transient => new TransientTypeResolver(type, key),
                Lifetime.Scoped => new ScopedTypeResolver(type, key, resolution),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unhandled lifetime in ContainerBuilder.RegisterType() method.")
            };

            return Add(type, contracts, key, resolver);
        }

        public ContainerBuilder RegisterValue(object value, object key = null)
        {
            return RegisterValue(value, new[] { value.GetType() }, key);
        }

        public ContainerBuilder RegisterValue(object value, Type[] contracts, object key = null)
        {
            Assert.IsTrue(contracts != null && contracts.Length > 0);
            var resolver = new SingletonValueResolver(value);
            return Add(value.GetType(), contracts, key, resolver);
        }

        public ContainerBuilder RegisterFactory<T>(Func<Container, T> factory, Lifetime lifetime = Lifetime.Singleton, Resolution resolution = Resolution.Lazy)
        {
            return RegisterFactory(factory, new[] { typeof(T) }, lifetime, resolution);
        }

        public ContainerBuilder RegisterFactory<T>(Func<Container, T> factory,
            Type[] contracts,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            Assert.IsNotNull(factory);
            Assert.IsTrue(contracts != null && contracts.Length > 0);
            Assert.IsFalse(lifetime == Lifetime.Transient && resolution == Resolution.Eager, "Factory registration Lifetime.Transient + Resolution.Eager not allowed");

            IResolver resolver = lifetime switch
            {
                Lifetime.Singleton => new SingletonFactoryResolver(TypelessFactory, resolution),
                Lifetime.Transient => new TransientFactoryResolver(TypelessFactory),
                Lifetime.Scoped => new ScopedFactoryResolver(TypelessFactory, resolution),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unhandled lifetime in ContainerBuilder.RegisterFactory() method.")
            };

            return Add(typeof(T), contracts, null, resolver);

            object TypelessFactory(Container container)
            {
                return factory.Invoke(container);
            }
        }

        private ContainerBuilder Add(Type concrete,
            Type[] contracts,
            object key,
            IResolver resolver)
        {
            var binding = Binding.Validated(resolver, concrete, key, contracts);
            Bindings.Add(binding);
            return this;
        }
    }
}