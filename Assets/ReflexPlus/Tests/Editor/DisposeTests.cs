using System;
using NUnit.Framework;
using ReflexPlus;
using ReflexPlus.Core;

namespace ReflexPlusEditor.Tests
{
    internal class DisposeTests
    {
        private class Service : IDisposable
        {
            public int Disposed { get; private set; }

            public void Dispose()
            {
                Disposed++;
            }
        }

        [Test]
        public void SingletonFromType_ShouldBeDisposedWhenOwnerIsDisposed_ReturnsCorrectValue()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(Service))
                .Build();

            var service = container.Single<Service>();
            container.Dispose();
            Assert.That(service.Disposed, Is.EqualTo(1));
        }

        [Test]
        public void SingletonFromValue_ShouldBeDisposedWhenOwnerIsDisposed_ReturnsCorrectValue()
        {
            var service = new Service();
            var container = new ContainerBuilder()
                .RegisterValue(service)
                .Build();

            container.Dispose();
            Assert.That(service.Disposed, Is.EqualTo(1));
        }

        [Test]
        public void SingletonFromFactory_ShouldBeDisposedWhenOwnerIsDisposed_ReturnsCorrectValue()
        {
            var container = new ContainerBuilder()
                .RegisterFactory(Factory)
                .Build();

            var service = container.Single<Service>();
            container.Dispose();
            Assert.That(service.Disposed, Is.EqualTo(1));
            return;

            Service Factory(Container ctx)
            {
                return new Service();
            }
        }

        [Test]
        public void TransientFromType_ShouldBeDisposedWhenOwnerIsDisposed_ReturnsCorrectValue()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(Service), Lifetime.Transient)
                .Build();

            var service = container.Single<Service>();
            container.Dispose();
            Assert.That(service.Disposed, Is.EqualTo(1));
        }

        [Test]
        public void TransientFromFactory_ShouldBeDisposedWhenOwnerIsDisposed_ReturnsCorrectValue()
        {
            var container = new ContainerBuilder()
                .RegisterFactory(Factory, Lifetime.Transient)
                .Build();

            var service = container.Single<Service>();
            container.Dispose();
            Assert.That(service.Disposed, Is.EqualTo(1));
            return;

            Service Factory(Container ctx)
            {
                return new Service();
            }
        }
    }
}