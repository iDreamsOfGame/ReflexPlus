using System;
using ReflexPlus.Exceptions;
using ReflexPlus.Resolvers;

namespace ReflexPlus.Core
{
    public sealed class Binding
    {
        private Binding()
        {
        }

        private Binding(IResolver resolver, Type contract, object key = null)
        {
            Resolver = resolver;
            Contract = contract;
            Key = key;
        }

        private Binding(IResolver resolver, Type[] contracts, object key = null)
        {
            Resolver = resolver;
            Contracts = contracts;
            Key = key;
        }

        public IResolver Resolver { get; }

        public Type Contract { get; }

        public Type[] Contracts { get; }

        public object Key { get; }

        public static Binding Validated(IResolver resolver,
            Type concrete,
            Type contract,
            object key = null)
        {
            if (!contract.IsAssignableFrom(concrete))
                throw new ContractDefinitionException(concrete, contract);
            
            return new Binding(resolver, contract, key);
        }

        public static Binding Validated(IResolver resolver,
            Type concrete,
            Type[] contracts,
            object key = null)
        {
            foreach (var contract in contracts)
            {
                if (!contract.IsAssignableFrom(concrete))
                {
                    throw new ContractDefinitionException(concrete, contract);
                }
            }

            return new Binding(resolver, contracts, key);
        }
    }
}