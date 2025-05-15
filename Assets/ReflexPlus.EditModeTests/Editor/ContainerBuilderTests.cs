using System;
using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;
using UnityEngine;

namespace ReflexPlus.EditModeTests
{
    internal class ContainerBuilderTests
    {
        private interface IValuable
        {
            int Value { get; set; }
        }

        private class Valuable : IValuable
        {
            public int Value { get; set; }
        }

        [Test]
        public void AddSingletonFromType_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterType(typeof(Valuable), new[] { typeof(IDisposable) });
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }

        [Test]
        public void AddSingletonFromType_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterType(typeof(Valuable), new[] { typeof(object), typeof(Valuable), typeof(IValuable) });
            addSingleton.Should().NotThrow();
        }

        [Test]
        public void AddSingletonFromValue_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterValue(new Valuable(), new[] { typeof(IDisposable) });
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }

        [Test]
        public void AddSingletonFromValue_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterValue(new Valuable(), new[] { typeof(object), typeof(Valuable), typeof(IValuable) });
            addSingleton.Should().NotThrow();
        }

        [Test]
        public void AddSingletonFromFactory_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterFactory(Factory, new[] { typeof(IDisposable) });
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
            return;

            Valuable Factory(Container container)
            {
                return new Valuable();
            }
        }

        [Test]
        public void AddSingletonFromFactory_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterFactory(Factory, new[] { typeof(object), typeof(Valuable), typeof(IValuable) });
            addSingleton.Should().NotThrow();
            return;

            Valuable Factory(Container container)
            {
                return new Valuable();
            }
        }

        [Test]
        public void AddTransientFromType_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterType(typeof(Valuable), new[] { typeof(IDisposable) }, Lifetime.Transient);
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
        }

        [Test]
        public void AddTransientFromType_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterType(typeof(Valuable), new[] { typeof(object), typeof(Valuable), typeof(IValuable) }, Lifetime.Transient);
            addSingleton.Should().NotThrow();
        }

        [Test]
        public void AddTransientFromFactory_ValuableWithIDisposableAsContract_ShouldThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterFactory(Factory, new[] { typeof(IDisposable) }, Lifetime.Transient);
            addSingleton.Should().ThrowExactly<ContractDefinitionException>();
            return;

            Valuable Factory(Container container)
            {
                return new Valuable();
            }
        }

        [Test]
        public void AddTransientFromFactory_ValuableWithObjectAndValuableAndIValuableAsContract_ShouldNotThrow()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterFactory(Factory, new[] { typeof(object), typeof(Valuable), typeof(IValuable) }, Lifetime.Transient);
            addSingleton.Should().NotThrow();
            return;

            Valuable Factory(Container container)
            {
                return new Valuable();
            }
        }

        [Test]
        public void HasBinding_ShouldTrue()
        {
            var value = Debug.unityLogger;
            var builder = new ContainerBuilder().RegisterValue(value);
            builder.HasBinding(value.GetType()).Should().BeTrue();
        }

        [Test]
        public void Build_CallBack_ShouldBeCalled()
        {
            Container container = null;
            var builder = new ContainerBuilder();
            builder.OnContainerBuilt += ContainerCallback;

            builder.Build();
            container.Should().NotBeNull();
            return;

            void ContainerCallback(Container ctx)
            {
                container = ctx;
            }
        }
    }
}