using System;
using System.Collections.Generic;
using System.Linq;
using ReflexPlus.Extensions;
using ReflexPlus.Exceptions;
using ReflexPlus.Injectors;
using ReflexPlus.Logging;
using ReflexPlus.Resolvers;

namespace ReflexPlus.Core
{
    public sealed class Container : IDisposable
    {
        public string Name { get; }

        internal Container Parent { get; }

        internal List<Container> Children { get; } = new();

        internal Dictionary<RegistrationId, List<IResolver>> ResolversByContract { get; }

        internal DisposableCollection Disposables { get; }

        internal Container(string name,
            Container parent,
            Dictionary<RegistrationId, List<IResolver>> resolversByContract,
            DisposableCollection disposables)
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
            ReflexPlusLogger.Log($"Container {Name} disposed", LogLevel.Info);
        }

        public Container Scope(Action<ContainerBuilder> extend = null)
        {
            var builder = new ContainerBuilder().SetParent(this);
            extend?.Invoke(builder);
            var scoped = builder.Build();
            return scoped;
        }
        
        public T Construct<T>(object attributeKey = null) => (T)Construct(typeof(T), attributeKey);

        public object Construct(Type concrete, object attributeKey = null)
        {
            var instance = ConstructorInjector.Construct(concrete, this);
            if (instance == null)
                return null;
            
            AttributeInjector.InjectInto(instance, attributeKey, this);
            return instance;
        }

        public object Resolve(Type type, object key) => Resolve(type, false, key);

        public object Resolve(Type type, bool optional = false, object key = null)
        {
            if (type.IsEnumerable(out var elementType))
            {
                return All(elementType).CastDynamic(elementType);
            }

            var resolvers = GetResolvers(type, optional, key);
            var lastResolver = resolvers.Last();
            var resolved = lastResolver.Resolve(this);
            return resolved;
        }

        public TContract Resolve<TContract>(object key) => Resolve<TContract>(false, key);

        public TContract Resolve<TContract>(bool optional = false, object key = null)
        {
            return (TContract)Resolve(typeof(TContract), optional, key);
        }

        public object Single(Type type, object key) => Single(type, false, key);

        public object Single(Type type, bool optional = false, object key = null)
        {
            return GetResolvers(type, optional, key).Single().Resolve(this);
        }

        public TContract Single<TContract>(object key) => Single<TContract>(false, key);

        public TContract Single<TContract>(bool optional = false, object key = null)
        {
            return (TContract)Single(typeof(TContract), optional, key);
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
                ? resolvers.Select(resolver => (TContract)resolver.Resolve(this)).ToArray()
                : Enumerable.Empty<TContract>();
        }

        private IEnumerable<IResolver> GetResolvers(Type contract, bool optional = false, object key = null)
        {
            var registrationId = new RegistrationId(contract, key);
            if (ResolversByContract.TryGetValue(registrationId, out var resolvers))
            {
                return resolvers;
            }

            if (!optional)
                throw new UnknownContractException(contract);

            return null;
        }

        private void OverrideSelfInjection()
        {
            ResolversByContract[new RegistrationId(typeof(Container))] = new List<IResolver> { new SingletonValueResolver(this) };
        }
    }
}