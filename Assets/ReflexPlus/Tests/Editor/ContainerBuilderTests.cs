using System;
using NUnit.Framework;
using ReflexPlus;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;
using UnityEngine;

namespace ReflexPlusEditor.Tests
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
        public void AddSingletonFromType_ValuableWithIDisposableAsContract_ThrowsContractDefinitionException()
        {
            Assert.Throws<ContractDefinitionException>(() => new ContainerBuilder().RegisterType(typeof(Valuable), new[] { typeof(IDisposable) }));
        }

        [Test]
        public void AddSingletonFromType_ValuableWithObjectAndValuableAndIValuableAsContract_DoesNotThrowAnyException()
        {
            var builder = new ContainerBuilder();
            Assert.DoesNotThrow(() => builder.RegisterType(typeof(Valuable), new[] { typeof(object), typeof(Valuable), typeof(IValuable) }));
        }

        [Test]
        public void AddSingletonFromValue_ValuableWithIDisposableAsContract_ThrowsContractDefinitionException()
        {
            Assert.Throws<ContractDefinitionException>(() => new ContainerBuilder().RegisterValue(new Valuable(), new[] { typeof(IDisposable) }));
        }

        [Test]
        public void AddSingletonFromValue_ValuableWithObjectAndValuableAndIValuableAsContract_DoesNotThrowAnyException()
        {
            Action addSingleton = () => new ContainerBuilder().RegisterValue(new Valuable(), new[] { typeof(object), typeof(Valuable), typeof(IValuable) });
            Assert.DoesNotThrow(() => addSingleton.Invoke());
        }

        [Test]
        public void AddSingletonFromFactory_ValuableWithIDisposableAsContract_ThrowsContractDefinitionException()
        {
            Assert.Throws<ContractDefinitionException>(() => new ContainerBuilder().RegisterFactory(Factory, new[] { typeof(IDisposable) }));
            return;

            Valuable Factory(Container container)
            {
                return new Valuable();
            }
        }

        [Test]
        public void AddSingletonFromFactory_ValuableWithObjectAndValuableAndIValuableAsContract_DoesNotThrowAnyException()
        {
            var builder = new ContainerBuilder();
            Action addSingleton = () => builder.RegisterFactory(Factory, new[] { typeof(object), typeof(Valuable), typeof(IValuable) });
            Assert.DoesNotThrow(() => addSingleton.Invoke());
            return;

            Valuable Factory(Container container)
            {
                return new Valuable();
            }
        }

        [Test]
        public void AddTransientFromType_ValuableWithIDisposableAsContract_ThrowsContractDefinitionException()
        {
            Assert.Throws<ContractDefinitionException>(() => new ContainerBuilder().RegisterType(typeof(Valuable), new[] { typeof(IDisposable) }, Lifetime.Transient));
        }

        [Test]
        public void AddTransientFromType_ValuableWithObjectAndValuableAndIValuableAsContract_DoesNotThrowAnyException()
        {
            var builder = new ContainerBuilder();
            Action addTransient = () => builder.RegisterType(typeof(Valuable), new[] { typeof(object), typeof(Valuable), typeof(IValuable) }, Lifetime.Transient);
            Assert.DoesNotThrow(() => addTransient.Invoke());
        }

        [Test]
        public void AddTransientFromFactory_ValuableWithIDisposableAsContract_ThrowsContractDefinitionException()
        {
            Assert.Throws<ContractDefinitionException>(() => new ContainerBuilder().RegisterFactory(Factory, new[] { typeof(IDisposable) }, Lifetime.Transient));
            return;

            Valuable Factory(Container container)
            {
                return new Valuable();
            }
        }

        [Test]
        public void AddTransientFromFactory_ValuableWithObjectAndValuableAndIValuableAsContract_DoesNotThrowAnyException()
        {
            var builder = new ContainerBuilder();
            Action addTransient = () => builder.RegisterFactory(Factory, new[] { typeof(object), typeof(Valuable), typeof(IValuable) }, Lifetime.Transient);
            Assert.DoesNotThrow(() => addTransient.Invoke());
            return;

            Valuable Factory(Container container)
            {
                return new Valuable();
            }
        }

        [Test]
        public void HasBinding_ReturnsTrue()
        {
            var value = SystemInfo.deviceType;
            var builder = new ContainerBuilder().RegisterValue(value);
            Assert.That(builder.HasBinding(value.GetType()), Is.True);
        }

        [Test]
        public void Unbind_ReturnsTrue()
        {
            var value = SystemInfo.deviceType;
            var builder = new ContainerBuilder().RegisterValue(value);
            Assert.That(builder.Unbind(value.GetType()), Is.True);
        }

        [Test]
        public void Unbind_RegisterValueWithKey_ReturnTrue()
        {
            const string key = nameof(SystemInfo.deviceType);
            var value = SystemInfo.deviceType;
            var builder = new ContainerBuilder().RegisterValue(value, key);
            Assert.That(builder.Unbind(value.GetType(), key), Is.True);
        }

        [Test]
        public void Unbind_RegisterValueWithContract_ReturnTrue()
        {
            var value = Debug.unityLogger;
            var builder = new ContainerBuilder().RegisterValue<ILogger>(value);
            Assert.That(builder.Unbind<ILogger>(), Is.True);
        }

        [Test]
        public void Unbind_RegisterValueWithContractAndKey_ReturnTrue()
        {
            const string key = nameof(Debug.unityLogger);
            var value = Debug.unityLogger;
            var builder = new ContainerBuilder().RegisterValue<ILogger>(value, key);
            Assert.That(builder.Unbind<ILogger>(key), Is.True);
        }

        [Test]
        public void Build_CallBack_ReturnsNotNull()
        {
            Container container = null;
            var builder = new ContainerBuilder();
            builder.OnContainerBuilt += ContainerCallback;

            builder.Build();
            Assert.That(container, Is.Not.Null);
            return;

            void ContainerCallback(Container ctx)
            {
                container = ctx;
            }
        }
    }
}