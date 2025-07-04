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

        public Container CreatedContainer { get; private set; }

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
            
            // Inherited resolvers from created container.
            if (CreatedContainer != null)
            {
                foreach (var (registrationId, resolvers) in CreatedContainer.ResolversByContract)
                {
                    resolversByContract[registrationId] = new List<IResolver>(resolvers);
                }
            }

            // Owned resolvers
            foreach (var binding in Bindings)
            {
                disposables.Add(binding.Resolver);

                if (binding.Contract != null)
                {
                    var registrationId = new RegistrationId(binding.Contract, binding.Key);
                    if (!resolversByContract.TryGetValue(registrationId, out var resolvers))
                    {
                        resolvers = new List<IResolver>();
                        resolversByContract.Add(registrationId, resolvers);
                    }

                    resolvers.Add(binding.Resolver);
                }
                else if (binding.Contracts != null)
                {
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
            CreatedContainer = container;
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

        public bool HasBinding<TContract>(object key = null)
        {
            return HasBinding(typeof(TContract), key);
        }

        public bool HasBinding(Type contract, object key = null)
        {
            return Bindings.Any(binding => binding.Key == key 
                                           && (binding.Contract == contract || (binding.Contracts != null && binding.Contracts.Contains(contract))));
        }

        public bool Unbind<TContract>(object key = null)
        {
            return Unbind(typeof(TContract), key);
        }

        public bool Unbind(Type contract, object key = null)
        {
            var result = Bindings.RemoveAll(binding => binding.Key == key 
                                                       && (binding.Contract == contract || (binding.Contracts != null && binding.Contracts.Contains(contract))));
            return result > 0;
        }

        public ContainerBuilder RegisterType<TConcrete>(Lifetime lifetime, Resolution resolution = Resolution.Lazy)
        {
            return RegisterType<TConcrete>(typeof(TConcrete), null, lifetime, resolution);
        }

        public ContainerBuilder RegisterType<TConcrete>(object key = null, Lifetime lifetime = Lifetime.Singleton, Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(typeof(TConcrete), key, lifetime, resolution);
        }
        
        public ContainerBuilder RegisterType<TConcrete>(Type contract,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(typeof(TConcrete), contract, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterType<TConcrete>(Type contract,
            object key = null,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(typeof(TConcrete), contract, key, lifetime, resolution);
        }

        public ContainerBuilder RegisterType<TConcrete>(Type[] contracts,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType<TConcrete>(contracts, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterType<TConcrete>(Type[] contracts,
            object key = null,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(typeof(TConcrete), contracts, key, lifetime, resolution);
        }

        public ContainerBuilder RegisterType<TConcrete, TContract>(Lifetime lifetime, Resolution resolution = Resolution.Lazy)
        {
            return RegisterType<TConcrete, TContract>(null, lifetime, resolution);
        }

        public ContainerBuilder RegisterType<TConcrete, TContract>(object key = null, Lifetime lifetime = Lifetime.Singleton, Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(typeof(TConcrete), typeof(TContract), key, lifetime, resolution);
        }

        public ContainerBuilder RegisterType(Type type,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(type, type, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterType(Type type,
            object key = null,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(type, type, key, lifetime, resolution);
        }

        public ContainerBuilder RegisterType(Type type,
            Type contract,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(type, contract, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterType(Type type,
            Type contract,
            object key = null,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            Assert.IsNotNull(type);
            Assert.IsTrue(contract != null);
            Assert.IsFalse(lifetime == Lifetime.Transient && resolution == Resolution.Eager, "Type registration Lifetime.Transient + Resolution.Eager not allowed");

            var resolver = GetResolver(type, key, lifetime, resolution);
            return Add(type, contract, key, resolver);
        }

        public ContainerBuilder RegisterType(Type type,
            Type[] contracts,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterType(type, contracts, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterType(Type type,
            Type[] contracts,
            object key = null,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            Assert.IsNotNull(type);
            Assert.IsTrue(contracts != null && contracts.Length > 0);
            Assert.IsFalse(lifetime == Lifetime.Transient && resolution == Resolution.Eager, "Type registration Lifetime.Transient + Resolution.Eager not allowed");

            var resolver = GetResolver(type, key, lifetime, resolution);
            return Add(type, contracts, key, resolver);
        }

        public ContainerBuilder RegisterValue<TContract>(object value, object key = null)
        {
            return RegisterValue(value, typeof(TContract), key);
        }

        public ContainerBuilder RegisterValue(object value, object key = null)
        {
            return RegisterValue(value, value.GetType(), key);
        }

        public ContainerBuilder RegisterValue(object value, Type contract, object key = null)
        {
            Assert.IsTrue(contract != null);
            var resolver = new SingletonValueResolver(value);
            return Add(value.GetType(), contract, key, resolver);
        }

        public ContainerBuilder RegisterValue(object value, Type[] contracts, object key = null)
        {
            Assert.IsTrue(contracts != null && contracts.Length > 0);
            var resolver = new SingletonValueResolver(value);
            return Add(value.GetType(), contracts, key, resolver);
        }

        public ContainerBuilder RegisterFactory<TConcrete>(Func<Container, TConcrete> factory,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterFactory(factory, typeof(TConcrete), null, lifetime, resolution);
        }

        public ContainerBuilder RegisterFactory<TConcrete>(Func<Container, TConcrete> factory,
            object key = null,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterFactory(factory, typeof(TConcrete), key, lifetime, resolution);
        }

        public ContainerBuilder RegisterFactory<TConcrete, TContract>(Func<Container, TConcrete> factory,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterFactory<TConcrete, TContract>(factory, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterFactory<TConcrete, TContract>(Func<Container, TConcrete> factory,
            object key = null,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterFactory(factory, typeof(TContract), key, lifetime, resolution);
        }

        public ContainerBuilder RegisterFactory<TConcrete>(Func<Container, TConcrete> factory,
            Type contract,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterFactory(factory, contract, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterFactory<TConcrete>(Func<Container, TConcrete> factory,
            Type contract,
            object key = null,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            Assert.IsNotNull(factory);
            Assert.IsTrue(contract != null);
            Assert.IsFalse(lifetime == Lifetime.Transient && resolution == Resolution.Eager, "Factory registration Lifetime.Transient + Resolution.Eager not allowed");

            var resolver = GetResolver(factory, lifetime, resolution);
            return Add(typeof(TConcrete), contract, key, resolver);
        }

        public ContainerBuilder RegisterFactory<TConcrete>(Func<Container, TConcrete> factory,
            Type[] contracts,
            Lifetime lifetime,
            Resolution resolution = Resolution.Lazy)
        {
            return RegisterFactory(factory, contracts, null, lifetime, resolution);
        }

        public ContainerBuilder RegisterFactory<TConcrete>(Func<Container, TConcrete> factory,
            Type[] contracts,
            object key = null,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            Assert.IsNotNull(factory);
            Assert.IsTrue(contracts != null && contracts.Length > 0);
            Assert.IsFalse(lifetime == Lifetime.Transient && resolution == Resolution.Eager, "Factory registration Lifetime.Transient + Resolution.Eager not allowed");

            var resolver = GetResolver(factory, lifetime, resolution);
            return Add(typeof(TConcrete), contracts, key, resolver);
        }

        private static IResolver GetResolver(Type type,
            object key,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            IResolver resolver = lifetime switch
            {
                Lifetime.Singleton => new SingletonTypeResolver(type, key, resolution),
                Lifetime.Transient => new TransientTypeResolver(type, key),
                Lifetime.Scoped => new ScopedTypeResolver(type, key, resolution),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unhandled lifetime in ContainerBuilder.RegisterType() method.")
            };

            return resolver;
        }

        private static IResolver GetResolver<TConcrete>(Func<Container, TConcrete> factory,
            Lifetime lifetime = Lifetime.Singleton,
            Resolution resolution = Resolution.Lazy)
        {
            IResolver resolver = lifetime switch
            {
                Lifetime.Singleton => new SingletonFactoryResolver(TypelessFactory, resolution),
                Lifetime.Transient => new TransientFactoryResolver(TypelessFactory),
                Lifetime.Scoped => new ScopedFactoryResolver(TypelessFactory, resolution),
                _ => throw new ArgumentOutOfRangeException(nameof(lifetime), lifetime, "Unhandled lifetime in ContainerBuilder.RegisterFactory() method.")
            };

            return resolver;

            object TypelessFactory(Container container)
            {
                return factory.Invoke(container);
            }
        }

        private ContainerBuilder Add(Type concrete,
            Type contract,
            object key,
            IResolver resolver)
        {
            var binding = Binding.Validated(resolver, concrete, contract, key);
            Bindings.Add(binding);
            return this;
        }

        private ContainerBuilder Add(Type concrete,
            Type[] contracts,
            object key,
            IResolver resolver)
        {
            var binding = Binding.Validated(resolver, concrete, contracts, key);
            Bindings.Add(binding);
            return this;
        }
    }
}