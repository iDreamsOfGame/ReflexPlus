using System;
using System.Collections.Generic;
using NUnit.Framework;
using ReflexPlus;
using ReflexPlus.Core;

namespace ReflexPlusEditor.Tests
{
    internal class ScopedTests
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
        public void ScopedFromType_ShouldReturnAlwaysSameInstanceWhenCalledFromSameContainer_ReturnsExpectedInstances()
        {
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Scoped).Build();
            var childContainer = parentContainer.Scope();
            Assert.That(parentContainer.Resolve<Service>(), Is.SameAs(parentContainer.Resolve<Service>()));
            Assert.That(childContainer.Resolve<Service>(), Is.SameAs(childContainer.Resolve<Service>()));
        }

        [Test]
        public void ScopedFromFactory_ShouldReturnAlwaysSameInstanceWhenCalledFromSameContainer_ReturnsExpectedInstances()
        {
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Scoped).Build();
            var childContainer = parentContainer.Scope();
            Assert.That(parentContainer.Resolve<Service>(), Is.SameAs(parentContainer.Resolve<Service>()));
            Assert.That(childContainer.Resolve<Service>(), Is.SameAs(childContainer.Resolve<Service>()));
        }

        [Test]
        public void ScopedFromType_NewInstanceShouldBeConstructedForEveryNewContainer_ReturnsExpectedValue()
        {
            var instances = new HashSet<Service>();
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Scoped).Build();
            var childContainer = parentContainer.Scope();
            instances.Add(parentContainer.Resolve<Service>());
            instances.Add(childContainer.Resolve<Service>());
            instances.Add(parentContainer.Resolve<Service>());
            instances.Add(childContainer.Resolve<Service>());
            Assert.That(instances.Count, Is.EqualTo(2));
        }

        [Test]
        public void ScopedFromFactory_NewInstanceShouldBeConstructedForEveryNewContainer_ReturnsExpectedValue()
        {
            var instances = new HashSet<Service>();
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Scoped).Build();
            var childContainer = parentContainer.Scope();
            instances.Add(parentContainer.Resolve<Service>());
            instances.Add(childContainer.Resolve<Service>());
            instances.Add(parentContainer.Resolve<Service>());
            instances.Add(childContainer.Resolve<Service>());
            Assert.That(instances.Count, Is.EqualTo(2));
        }

        [Test]
        public void ScopedFromType_ConstructedInstancesShouldBeDisposedWithinConstructingContainer_ReturnsExpectedValues()
        {
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Scoped).Build();
            var childContainer = parentContainer.Scope();

            var instanceConstructedByChild = childContainer.Resolve<Service>();
            var instanceConstructedByParent = parentContainer.Resolve<Service>();

            childContainer.Dispose();

            Assert.That(instanceConstructedByChild.IsDisposed, Is.True);
            Assert.That(instanceConstructedByParent.IsDisposed, Is.False);
        }

        [Test]
        public void ScopedFromFactory_ConstructedInstancesShouldBeDisposedWithinConstructingContainer_ReturnsExpectedValues()
        {
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Scoped).Build();
            var childContainer = parentContainer.Scope();

            var instanceConstructedByChild = childContainer.Resolve<Service>();
            var instanceConstructedByParent = parentContainer.Resolve<Service>();

            childContainer.Dispose();

            Assert.That(instanceConstructedByChild.IsDisposed, Is.True);
            Assert.That(instanceConstructedByParent.IsDisposed, Is.False);
        }

        [Test, Retry(3)]
        public void ScopedFromType_ConstructedInstancesShouldBeCollectedWhenConstructingContainerIsDisposed_ReturnsExpectedValues()
        {
            WeakReference instanceConstructedByChild;
            WeakReference instanceConstructedByParent;
            var parentContainer = new ContainerBuilder().RegisterType(typeof(Service), Lifetime.Scoped).Build();

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
        public void ScopedFromFactory_ConstructedInstancesShouldBeCollectedWhenConstructingContainerIsDisposed_ReturnsExpectedValues()
        {
            WeakReference instanceConstructedByChild;
            WeakReference instanceConstructedByParent;
            var parentContainer = new ContainerBuilder().RegisterFactory(_ => new Service(), Lifetime.Scoped).Build();

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