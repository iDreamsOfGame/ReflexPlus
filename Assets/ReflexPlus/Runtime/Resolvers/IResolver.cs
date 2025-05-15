using System;
using ReflexPlus.Core;

namespace ReflexPlus.Resolvers
{
    public interface IResolver : IDisposable
    {
        Lifetime Lifetime { get; }

        Resolution Resolution { get; }

        object Resolve(Container container);
    }
}