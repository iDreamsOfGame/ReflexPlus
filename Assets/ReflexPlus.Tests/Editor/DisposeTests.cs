using System;
using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Core;

namespace ReflexPlus.EditModeTests
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
        public void SingletonFromType_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(Service))
                .Build();

            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
        }

        [Test]
        public void SingletonFromValue_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            var service = new Service();
            var container = new ContainerBuilder()
                .RegisterValue(service)
                .Build();

            container.Dispose();
            service.Disposed.Should().Be(1);
        }

        [Test]
        public void SingletonFromFactory_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            var container = new ContainerBuilder()
                .RegisterFactory(Factory)
                .Build();

            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
            return;

            Service Factory(Container ctx)
            {
                return new Service();
            }
        }

        [Test]
        public void TransientFromType_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(Service), Lifetime.Transient)
                .Build();

            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
        }

        [Test]
        public void TransientFromFactory_ShouldBeDisposed_WhenOwnerIsDisposed()
        {
            var container = new ContainerBuilder()
                .RegisterFactory(Factory, Lifetime.Transient)
                .Build();

            var service = container.Single<Service>();
            container.Dispose();
            service.Disposed.Should().Be(1);
            return;

            Service Factory(Container ctx)
            {
                return new Service();
            }
        }
    }
}