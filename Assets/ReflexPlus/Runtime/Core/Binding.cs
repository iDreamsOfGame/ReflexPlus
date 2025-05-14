using System;
using Reflex.Exceptions;
using Reflex.Resolvers;

namespace Reflex.Core
{
    public sealed class Binding
    {
        public IResolver Resolver { get; }
        
        public Type[] Contracts { get; }

        public object Key { get; }

        private Binding()
        {
        }

        private Binding(IResolver resolver, Type[] contracts, object key = null)
        {
            Resolver = resolver;
            Contracts = contracts;
            Key = key;
        }

        public static Binding Validated(IResolver resolver, Type concrete, object key = null, params Type[] contracts)
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