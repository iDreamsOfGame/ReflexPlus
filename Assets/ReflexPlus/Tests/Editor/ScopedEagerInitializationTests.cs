using NUnit.Framework;
using ReflexPlus;
using ReflexPlus.Core;
using UnityEngine;

namespace ReflexPlusEditor.Tests
{
    internal class ScopedEagerInitializationTests
    {
        private sealed class ScopedService
        {
            public ScopedService()
            {
                Debug.Log($"ScopedService {GetHashCode()} created.");
            }
        }

        [Test]
        public void Foo()
        {
            var containerBuilder = new ContainerBuilder().RegisterType(typeof(ScopedService), Lifetime.Scoped);

            var container = containerBuilder.Build();
            container.Scope();
            var scoped2 = container.Scope();
            var scoped3 = scoped2.Scope();
            scoped3.Scope();
        }
    }
}