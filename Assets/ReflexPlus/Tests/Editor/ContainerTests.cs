using System;
using System.Collections.Generic;
using NUnit.Framework;
using ReflexPlus;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

// ReSharper disable NotAccessedField.Local

namespace ReflexPlusEditor.Tests
{
    internal class ContainerTests
    {
        private interface IValuable
        {
            int Value { get; set; }
        }

        private class Valuable : IValuable
        {
            public int Value { get; set; }
        }

        private interface IClassWithDependency
        {
        }

        private class ClassWithDependency : IClassWithDependency
        {
            private readonly IValuable valuable;

            public ClassWithDependency(IValuable valuable)
            {
                this.valuable = valuable;
            }
        }

        [Test]
        public void Construct_GetInstanceOfUnregisteredType_ReturnsNotNull()
        {
            var container = new ContainerBuilder().Build();
            var valuable = container.Construct<Valuable>();
            Assert.That(valuable, Is.Not.Null);
        }

        [Test]
        public void Resolve_ValueTypeSingleton_ReturnsCorrectValue()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .Build();

            Assert.That(container.Single<int>(), Is.EqualTo(42));
        }

        [Test]
        public void Resolve_UninstalledValueType_ThrowsUnknownContractException()
        {
            Assert.Throws<UnknownContractException>(() => new ContainerBuilder().Build().Single<int>());
        }

        [Test]
        public void Resolve_AsTransientFromType_ReturnsNewInstance()
        {
            var container = new ContainerBuilder()
                .RegisterType<Valuable>(new[] { typeof(IValuable) }, Lifetime.Transient)
                .Build();

            container.Single<IValuable>().Value = 123;
            Assert.That(container.Single<IValuable>().Value, Is.EqualTo(0));
        }

        [Test]
        public void Resolve_AsTransientFromFactory_ReturnsCorrectCalls()
        {
            const string text = "Hello World!";
            var callbackAssertion = new CallbackAssertion();

            var container = new ContainerBuilder()
                .RegisterFactory(Factory, Lifetime.Transient)
                .Build();
            
            Assert.That(container.Single<string>(), Is.EqualTo(text));
            Assert.That(container.Single<string>(), Is.EqualTo(text));
            Assert.That(container.Single<string>(), Is.EqualTo(text));
            callbackAssertion.ShouldHaveBeenCalled(3);
            return;

            string Factory(Container ctx)
            {
                callbackAssertion.Invoke();
                return text;
            }
        }

        [Test]
        public void Resolve_AsSingletonFromType_ReturnsSameInstance()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(Valuable), new[] { typeof(IValuable) })
                .Build();

            container.Single<IValuable>().Value = 123;
            Assert.That(container.Single<IValuable>().Value, Is.EqualTo(123));
        }

        [Test]
        public void Resolve_AsSingletonFromFactory_ReturnsCorrectCalls()
        {
            const string text = "Hello World!";
            var callbackAssertion = new CallbackAssertion();

            var container = new ContainerBuilder()
                .RegisterFactory(Factory)
                .Build();
            
            Assert.That(container.Single<string>(), Is.EqualTo(text));
            Assert.That(container.Single<string>(), Is.EqualTo(text));
            Assert.That(container.Single<string>(), Is.EqualTo(text));
            callbackAssertion.ShouldHaveBeenCalledOnce();
            return;

            string Factory(Container ctx)
            {
                callbackAssertion.Invoke();
                return text;
            }
        }

        [Test]
        public void Resolve_UnknownDependency_ThrowsUnknownContractException()
        {
            Assert.Throws<UnknownContractException>(() => new ContainerBuilder().Build().Single<IValuable>());
        }

        [Test]
        public void Resolve_KnownDependencyAsTransientWithUnknownDependency_ThrowsConstructorInjectorException()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(ClassWithDependency), new[] { typeof(IClassWithDependency) }, Lifetime.Transient)
                .Build();
            
            Assert.Throws<ConstructorInjectorException>(() => container.Single<IClassWithDependency>());
        }

        [Test]
        public void Resolve_KnownDependencyAsSingletonWithUnknownDependency_ThrowsConstructorInjectorException()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(ClassWithDependency), new[] { typeof(IClassWithDependency) })
                .Build();
            
            Assert.Throws<ConstructorInjectorException>(() => container.Single<IClassWithDependency>());
        }

        [Test]
        public void Resolve_ValueTypeAsTransient_ReturnsDefaultValue()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(int), Lifetime.Transient)
                .Build();

            Assert.That(container.Single<int>(), Is.EqualTo(0));
        }

        private struct MyStruct
        {
            public readonly int Value;

            public MyStruct(int value)
            {
                Value = value;
            }
        }

        [Test]
        public void Resolve_ValueTypeAsTransient_CustomConstructor_ReturnsCorrectValue()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .RegisterType(typeof(MyStruct), Lifetime.Transient)
                .Build();

            Assert.That(container.Single<MyStruct>().Value, Is.EqualTo(42));
        }

        private interface ISetup<T>
        {
            void Setup(ref T instance);
        }

        private class IntSetup : ISetup<int>
        {
            public void Setup(ref int instance)
            {
                instance = 42;
            }
        }

        private class StringSetup : ISetup<string>
        {
            public void Setup(ref string instance)
            {
                instance = "abc";
            }
        }

        private class NumberAndStringCache
        {
            public readonly int Int;

            public readonly string String;

            public NumberAndStringCache(ISetup<int> intSetup, ISetup<string> stringSetup)
            {
                Int = 0;
                String = string.Empty;
                intSetup.Setup(ref Int);
                stringSetup.Setup(ref String);
            }
        }

        [Test]
        public void Resolve_ClassWithGenericDependency_WithNormalDefinition_ReturnsCorrectValues()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(NumberAndStringCache), Lifetime.Transient)
                .RegisterType(typeof(IntSetup), new[] { typeof(ISetup<int>) }, Lifetime.Transient)
                .RegisterType(typeof(StringSetup), new[] { typeof(ISetup<string>) }, Lifetime.Transient)
                .Build();

            var instance = container.Construct<NumberAndStringCache>();
            Assert.That(instance.Int, Is.EqualTo(42));
            Assert.That(instance.String, Is.EqualTo("abc"));
        }

        [Test]
        public void AddSingleton_WithoutContract_ReturnsCorrectValue()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .Build();

            Assert.That(container.Single<int>(), Is.EqualTo(42));
        }

        [Test]
        public void ResolveAll_WithoutMatch_ReturnsEmpty()
        {
            Assert.That(new ContainerBuilder().Build().All<IDisposable>(), Is.Empty);
        }

        [Test]
        public void All_OnParentShouldNotBeAffectedByScoped_ReturnsCorrectValues()
        {
            var container = new ContainerBuilder().RegisterValue(1).Build();
            Assert.That(string.Join(",", container.All<int>()), Is.EqualTo("1"));
            container.Scope(containerBuilder => { containerBuilder.RegisterValue(2); });
            Assert.That(string.Join(",", container.All<int>()), Is.EqualTo("1"));
        }

        [Test]
        public void HasBindingReturnFalseWhenBindingIsNotDefined_ReturnsFalse()
        {
            Assert.That(new ContainerBuilder().Build().HasBinding<int>(), Is.False);
        }

        [Test]
        public void HasBindingReturnTrueWhenBindingIsDefined_ReturnsTrue()
        {
            Assert.That(new ContainerBuilder().RegisterValue(42).Build().HasBinding<int>(), Is.True);
        }

        [Test]
        public void Unbind_ReturnFalseWhenBindingIsNotDefined_ReturnsFalse()
        {
            Assert.That(new ContainerBuilder().Build().Unbind(typeof(int)), Is.False);
        }
        
        [Test]
        public void Unbind_ReturnTrueWhenBindingIsDefined_ReturnsTrue()
        {
            Assert.That(new ContainerBuilder().RegisterValue(42).Build().Unbind<int>(), Is.True);
        }

        [Test]
        public void Unbind_RegisterValueWithKey_ReturnTrueWhenBindingIsDefined_ReturnsTrue()
        {
            const int value = 42;
            Assert.That(new ContainerBuilder().RegisterValue(value, value).Build().Unbind<int>(value), Is.True);
        }
        
        [Test]
        public void Unbind_RegisterValueWithContract_ReturnTrueWhenBindingIsDefined_ReturnsTrue()
        {
            Assert.That(new ContainerBuilder().RegisterValue<IList<int>>(new List<int> { 1, 2, 3 }).Build().Unbind<IList<int>>(), Is.True);
        }

        [Test]
        public void Unbind_RegisterValueWithContractAndKey_ReturnTrueWhenBindingIsDefined_ReturnsTrue()
        {
            const string key = "numbers";
            var container = new ContainerBuilder().RegisterValue<IList<int>>(new List<int> { 1, 2, 3 }, key).Build();
            Assert.That(container.Unbind<IList<int>>(key), Is.True);
        }
    }
}