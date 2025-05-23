using System;
using NUnit.Framework;
using ReflexPlus;
using ReflexPlus.Core;

namespace ReflexPlusEditor.Tests
{
    internal class TransientTests
    {
        private class Service : IDisposable
        {
            public bool IsDisposed { get; private set; }

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        [Test]
        public void TransientFromType_ConstructedInstances_ShouldBeDisposed_WithinConstructingContainer()
        {
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Transient).Build();
            var childContainer = parentContainer.Scope();

            var instanceConstructedByChild = childContainer.Resolve<Service>();
            var instanceConstructedByParent = parentContainer.Resolve<Service>();

            childContainer.Dispose();

            Assert.That(instanceConstructedByChild.IsDisposed, Is.True);
            Assert.That(instanceConstructedByParent.IsDisposed, Is.False);
        }

        [Test]
        public void TransientFromFactory_ConstructedInstances_ShouldBeDisposed_WithinConstructingContainer()
        {
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Transient).Build();
            var childContainer = parentContainer.Scope();

            var instanceConstructedByChild = childContainer.Resolve<Service>();
            var instanceConstructedByParent = parentContainer.Resolve<Service>();

            childContainer.Dispose();
            
            Assert.That(instanceConstructedByChild.IsDisposed, Is.True);
            Assert.That(instanceConstructedByParent.IsDisposed, Is.False);
        }

        [Test, Retry(3)]
        public void TransientFromType_ConstructedInstances_ShouldBeCollected_WhenConstructingContainerIsDisposed()
        {
            WeakReference instanceConstructedByChild;
            WeakReference instanceConstructedByParent;
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Transient).Build();

            Act();
            GarbageCollectionTests.ForceGarbageCollection();
            Assert.That(instanceConstructedByChild.IsAlive, Is.False);
            Assert.That(instanceConstructedByParent.IsAlive, Is.True);
            return;

            void Act()
            {
                using var childContainer = parentContainer.Scope();
                instanceConstructedByChild = new WeakReference(childContainer.Resolve<Service>());
                instanceConstructedByParent = new WeakReference(parentContainer.Resolve<Service>());
            }
        }

        [Test, Retry(3)]
        public void TransientFromFactory_ConstructedInstances_ShouldBeCollected_WhenConstructingContainerIsDisposed()
        {
            WeakReference instanceConstructedByChild;
            WeakReference instanceConstructedByParent;
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Transient).Build();

            Act();
            GarbageCollectionTests.ForceGarbageCollection();
            Assert.That(instanceConstructedByChild.IsAlive, Is.False);
            Assert.That(instanceConstructedByParent.IsAlive, Is.True);
            return;

            void Act()
            {
                using var childContainer = parentContainer.Scope();
                instanceConstructedByChild = new WeakReference(childContainer.Resolve<Service>());
                instanceConstructedByParent = new WeakReference(parentContainer.Resolve<Service>());
            }
        }
    }
}