using System;
using System.Collections.Generic;
using System.Linq;
using Reflex.Exceptions;
using Reflex.Extensions;
using Reflex.Generics;
using Reflex.Injectors;
using Reflex.Logging;
using Reflex.Registration;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public sealed class Container : IDisposable
    {
        public string Name { get; }
        internal Container Parent { get; }
        internal List<Container> Children { get; } = new();
        internal Dictionary<RegistrationId, List<IResolver>> ResolversByContract { get; }
        internal DisposableCollection Disposables { get; }
        
        internal Container(string name, Container parent, Dictionary<RegistrationId, List<IResolver>> resolversByContract, DisposableCollection disposables)
        {
            Diagnosis.RegisterBuildCallSite(this);
            Name = name;
            Parent = parent;
            Parent?.Children.Add(this);
            ResolversByContract = resolversByContract;
            Disposables = disposables;
            OverrideSelfInjection();
        }

        public bool HasBinding<T>(object key = null)
        {
            return HasBinding(typeof(T), key);
        }

        public bool HasBinding(Type type, object key = null)
        {
            var registrationId = new RegistrationId(type, key);
            return ResolversByContract.ContainsKey(registrationId);
        }

        public void Dispose()
        {
            foreach (var child in Children.Reversed())
            {
                child.Dispose();
            }

            Parent?.Children.Remove(this);
            ResolversByContract.Clear();
            Disposables.Dispose();
            ReflexLogger.Log($"Container {Name} disposed", LogLevel.Info);
        }

        public Container Scope(Action<ContainerBuilder> extend = null)
        {
            var builder = new ContainerBuilder().SetParent(this);
            extend?.Invoke(builder);
            var scoped = builder.Build();
            return scoped;
        }
        
        public T Construct<T>(object key = null)
        {
            return (T)Construct(typeof(T), key);
        }

        public object Construct(Type concrete, object key = null)
        {
            var instance = ConstructorInjector.Construct(concrete, this);
            AttributeInjector.Inject(instance, key, this);
            return instance;
        }
        
        public object Resolve(Type type, object key = null)
        {
            if (type.IsEnumerable(out var elementType))
            {
                return All(elementType).CastDynamic(elementType);
            }

            var resolvers = GetResolvers(type, key);
            var lastResolver = resolvers.Last();
            var resolved = lastResolver.Resolve(this);
            return resolved;
        }

        public TContract Resolve<TContract>(object key = null)
        {
            return (TContract)Resolve(typeof(TContract), key);
        }
        
        public object Single(Type type, object key = null)
        {
            return GetResolvers(type, key).Single().Resolve(this);
        }

        public TContract Single<TContract>(object key = null)
        {
            return (TContract)Single(typeof(TContract), key);
        }

        public IEnumerable<object> All(Type contract, object key = null)
        {
            var registrationId = new RegistrationId(contract, key);
            return ResolversByContract.TryGetValue(registrationId, out var resolvers)
                ? resolvers.Select(resolver => resolver.Resolve(this)).ToArray()
                : Enumerable.Empty<object>();
        }

        public IEnumerable<TContract> All<TContract>(object key = null)
        {
            var registrationId = new RegistrationId(typeof(TContract), key);
            return ResolversByContract.TryGetValue(registrationId, out var resolvers)
                ? resolvers.Select(resolver => (TContract) resolver.Resolve(this)).ToArray()
                : Enumerable.Empty<TContract>();
        }

        private IEnumerable<IResolver> GetResolvers(Type contract, object key = null)
        {
            var registrationId = new RegistrationId(contract, key);
            if (ResolversByContract.TryGetValue(registrationId, out var resolvers))
            {
                return resolvers;
            }

            throw new UnknownContractException(contract);
        }
        
        private void OverrideSelfInjection()
        {
            ResolversByContract[new RegistrationId(typeof(Container))] = new List<IResolver> { new SingletonValueResolver(this) };
        }
    }
}