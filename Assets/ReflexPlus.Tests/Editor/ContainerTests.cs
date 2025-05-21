using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

// ReSharper disable NotAccessedField.Local

namespace ReflexPlus.EditModeTests
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
        public void Resolve_ValueTypeSingleton_ShouldReturn42()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .Build();

            container.Single<int>().Should().Be(42);
        }

        [Test]
        public void Resolve_UninstalledValueType_ShouldThrowUnknownContractException()
        {
            var container = new ContainerBuilder().Build();
            Action resolve = () => container.Single<int>();
            resolve.Should().Throw<UnknownContractException>();
        }

        [Test]
        public void Resolve_AsTransientFromType_ShouldReturnAlwaysANewInstance()
        {
            var container = new ContainerBuilder()
                .RegisterType<Valuable>(new[] { typeof(IValuable) }, Lifetime.Transient)
                .Build();

            container.Single<IValuable>().Value = 123;
            container.Single<IValuable>().Value.Should().Be(0);
        }

        [Test]
        public void Resolve_AsTransientFromFactory_ShouldRunFactoryAlways()
        {
            var callbackAssertion = new CallbackAssertion();

            string Factory(Container container)
            {
                callbackAssertion.Invoke();
                return "Hello World!";
            }

            var container = new ContainerBuilder()
                .RegisterFactory(Factory, Lifetime.Transient)
                .Build();

            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            callbackAssertion.ShouldHaveBeenCalled(3);
        }

        [Test]
        public void Resolve_AsSingletonFromType_ShouldReturnAlwaysSameInstance()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(Valuable), new[] { typeof(IValuable) })
                .Build();

            container.Single<IValuable>().Value = 123;
            container.Single<IValuable>().Value.Should().Be(123);
        }

        [Test]
        public void Resolve_AsSingletonFromFactory_ShouldRunFactoryOnce()
        {
            var callbackAssertion = new CallbackAssertion();

            var container = new ContainerBuilder()
                .RegisterFactory(Factory)
                .Build();

            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            container.Single<string>().Should().Be("Hello World!");
            callbackAssertion.ShouldHaveBeenCalledOnce();
            return;

            string Factory(Container ctx)
            {
                callbackAssertion.Invoke();
                return "Hello World!";
            }
        }

        [Test]
        public void Resolve_UnknownDependency_ShouldThrowUnknownContractException()
        {
            var container = new ContainerBuilder().Build();
            Action resolve = () => container.Single<IValuable>();
            resolve.Should().Throw<UnknownContractException>();
        }

        [Test]
        public void Resolve_KnownDependencyAsTransientWithUnknownDependency_ShouldThrowConstructorInjectorException()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(ClassWithDependency), new[] { typeof(IClassWithDependency) }, Lifetime.Transient)
                .Build();

            Action resolve = () => container.Single<IClassWithDependency>();
            resolve.Should().Throw<ConstructorInjectorException>();
        }

        [Test]
        public void Resolve_KnownDependencyAsSingletonWithUnknownDependency_ShouldThrowConstructorInjectorException()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(ClassWithDependency), new[] { typeof(IClassWithDependency) })
                .Build();

            Action resolve = () => container.Single<IClassWithDependency>();
            resolve.Should().Throw<ConstructorInjectorException>();
        }

        [Test]
        public void Resolve_ValueTypeAsTransient_ShouldReturnDefault()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(int), Lifetime.Transient)
                .Build();

            container.Single<int>().Should().Be(0);
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
        public void Resolve_ValueTypeAsTransient_CustomConstructor_ValueShouldReturn42()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .RegisterType(typeof(MyStruct), Lifetime.Transient)
                .Build();

            container.Single<MyStruct>().Value.Should().Be(42);
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
        public void Resolve_ClassWithGenericDependency_WithNormalDefinition_ValuesShouldBe42AndABC()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(NumberAndStringCache), Lifetime.Transient)
                .RegisterType(typeof(IntSetup), new[] { typeof(ISetup<int>) }, Lifetime.Transient)
                .RegisterType(typeof(StringSetup), new[] { typeof(ISetup<string>) }, Lifetime.Transient)
                .Build();

            var instance = container.Construct<NumberAndStringCache>();
            instance.Int.Should().Be(42);
            instance.String.Should().Be("abc");
        }

        [Test]
        public void AddSingleton_WithoutContract_ShouldBindToItsType()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .Build();

            container.Single<int>().Should().Be(42);
        }

        [Test]
        public void ResolveAll_WithoutMatch_ShouldReturnEmptyEnumerable()
        {
            var container = new ContainerBuilder().Build();
            container.All<IDisposable>().Should().BeEmpty();
        }

        [Test]
        public void All_OnParentShouldNotBeAffectedByScoped()
        {
            var container = new ContainerBuilder().RegisterValue(1).Build();
            string.Join(",", container.All<int>()).Should().Be("1");
            container.Scope(containerBuilder => { containerBuilder.RegisterValue(2); });
            string.Join(",", container.All<int>()).Should().Be("1");
        }

        [Test]
        public void HasBindingReturnFalseWhenBindingIsNotDefined()
        {
            var container = new ContainerBuilder().Build();
            container.HasBinding<int>().Should().BeFalse();
        }

        [Test]
        public void HasBindingReturnTrueWhenBindingIsDefined()
        {
            var container = new ContainerBuilder().RegisterValue(42).Build();
            container.HasBinding<int>().Should().BeTrue();
        }

        [Test]
        public void Unbind_ReturnFalseWhenBindingIsNotDefined()
        {
            var container = new ContainerBuilder().Build();
            container.Unbind(typeof(int)).Should().BeFalse();
        }
        
        [Test]
        public void Unbind_ReturnTrueWhenBindingIsDefined()
        {
            var container = new ContainerBuilder().RegisterValue(42).Build();
            container.Unbind<int>().Should().BeTrue();
        }

        [Test]
        public void Unbind_RegisterValueWithKey_ReturnTrueWhenBindingIsDefined()
        {
            const string key = "42";
            var container = new ContainerBuilder().RegisterValue(42, key).Build();
            container.Unbind<int>(key).Should().BeTrue();
        }
        
        [Test]
        public void Unbind_RegisterValueWithContract_ReturnTrueWhenBindingIsDefined()
        {
            var container = new ContainerBuilder().RegisterValue<IList<int>>(new List<int> { 1, 2, 3 }).Build();
            container.Unbind<IList<int>>().Should().BeTrue();
        }

        [Test]
        public void Unbind_RegisterValueWithContractAndKey_ReturnTrueWhenBindingIsDefined()
        {
            const string key = "numbers";
            var container = new ContainerBuilder().RegisterValue<IList<int>>(new List<int> { 1, 2, 3 }, key).Build();
            container.Unbind<IList<int>>(key).Should().BeTrue();
        }
    }
}