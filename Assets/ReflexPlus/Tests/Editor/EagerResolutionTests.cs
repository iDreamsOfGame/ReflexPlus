using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using ReflexPlus;
using ReflexPlus.Core;

namespace ReflexPlusEditor.Tests
{
    internal class EagerResolutionTests
    {
        public class Service
        {
            public static event Action<Service> OnServiceConstructed;

            public Service()
            {
                OnServiceConstructed?.Invoke(this);
            }
        }

        [Test]
        public void EagerResolution_EagerSingletonShouldBeConstructedOnContainerBuild_ReturnsSameInstance()
        {
            var eagerlyConstructedServices = new List<Service>();
            Service.OnServiceConstructed += OnServiceConstructed;

            var container = new ContainerBuilder()
                .RegisterType<Service>(Lifetime.Singleton, Resolution.Eager)
                .Build();

            Service.OnServiceConstructed -= OnServiceConstructed;
            Assert.That(eagerlyConstructedServices, Is.Not.Empty);
            Assert.That(eagerlyConstructedServices.Single(), Is.Not.Null);
            Assert.That(eagerlyConstructedServices.Single(), Is.SameAs(container.Resolve<Service>()));
            return;

            void OnServiceConstructed(Service constructedService)
            {
                eagerlyConstructedServices.Add(constructedService);
            }
        }

        [Test]
        public void EagerResolution_EagerSingletonShouldBeConstructedOnlyOnce_ReturnsSameInstance()
        {
            var eagerlyConstructedServices = new List<Service>();
            Service.OnServiceConstructed += OnServiceConstructed;

            var container = new ContainerBuilder()
                .RegisterType(typeof(Service), Lifetime.Singleton, Resolution.Eager)
                .Build();

            var scoped1 = container.Scope();
            var scoped2 = scoped1.Scope();

            Service.OnServiceConstructed -= OnServiceConstructed;
            Assert.That(eagerlyConstructedServices.Count, Is.EqualTo(1));
            Assert.That(eagerlyConstructedServices.Single(), Is.SameAs(scoped2.Resolve<Service>()));
            return;

            void OnServiceConstructed(Service constructedService)
            {
                eagerlyConstructedServices.Add(constructedService);
            }
        }

        [Test]
        public void EagerResolution_EagerScopedShouldBeConstructedOncePerContainer_ReturnsSameInstances()
        {
            var eagerlyConstructedServices = new List<Service>();
            Service.OnServiceConstructed += OnServiceConstructed;

            var container = new ContainerBuilder()
                .RegisterType(typeof(Service), Lifetime.Scoped, Resolution.Eager)
                .Build();

            var scoped1 = container.Scope();
            var scoped2 = scoped1.Scope();

            Service.OnServiceConstructed -= OnServiceConstructed;
            Assert.That(eagerlyConstructedServices.Count, Is.EqualTo(3));
            Assert.That(eagerlyConstructedServices[0], Is.SameAs(container.Resolve<Service>()));
            Assert.That(eagerlyConstructedServices[1], Is.SameAs(scoped1.Resolve<Service>()));
            Assert.That(eagerlyConstructedServices[2], Is.SameAs(scoped2.Resolve<Service>()));
            return;

            void OnServiceConstructed(Service constructedService)
            {
                eagerlyConstructedServices.Add(constructedService);
            }
        }
    }
}