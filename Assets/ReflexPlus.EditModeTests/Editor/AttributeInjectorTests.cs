using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Attributes;
using ReflexPlus.Core;

// ReSharper disable ClassNeverInstantiated.Local

namespace ReflexPlus.EditModeTests
{
    internal class AttributeInjectorTests
    {
        private class Foo
        {
            [Inject]
            public readonly int InjectedFieldValue;

            [Inject]
            public int InjectedPropertyValue { get; private set; }

            public int InjectedMethodValue { get; private set; }

            [Inject]
            private void Inject(int value)
            {
                InjectedMethodValue = value;
            }
        }
        
        private class FooInner
        {
        }

        private class Foo2
        {
            [Inject]
            private readonly string injectedFieldValue;

            [Inject("other")]
            private readonly string injectedFieldValueWithKey;

            [Inject]
            private readonly FooInner injectFieldType;

            // [Inject("other")]
            // private readonly FooInner injectFieldTypeWithKey;

            public string InjectedFieldValue => injectedFieldValue;

            public string InjectedFieldValueWithKey => injectedFieldValueWithKey;

            public FooInner InjectFieldType => injectFieldType;

            // public FooInner InjectFieldTypeWithKey => injectFieldTypeWithKey;
        }

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnFieldsPropertiesAndMethodsMarkedWithInjectAttribute()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .RegisterType(typeof(Foo))
                .Build();

            var foo = container.Single<Foo>();
            foo.InjectedFieldValue.Should().Be(42);
            foo.InjectedPropertyValue.Should().Be(42);
            foo.InjectedMethodValue.Should().Be(42);
        }

        [Test]
        public void AddTransient_ShouldRunAttributeInjectionOnFieldsPropertiesAndMethodsMarkedWithInjectAttribute()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .RegisterType(typeof(Foo))
                .Build();

            var foo = container.Single<Foo>();
            foo.InjectedFieldValue.Should().Be(42);
            foo.InjectedPropertyValue.Should().Be(42);
            foo.InjectedMethodValue.Should().Be(42);
        }

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnFieldsMarkedByInjectAttributeWithKey()
        {
            var container = new ContainerBuilder()
                .RegisterValue("42")
                .RegisterValue("43", "other")
                // .RegisterType<FooInner>()
                // .RegisterType(typeof(FooInner), "other")
                .RegisterType<Foo2>()
                .Build();

            var foo = container.Single<Foo2>();
            Assert.NotNull(foo);
            // Assert.AreNotEqual(foo.InjectFieldType, foo.InjectFieldTypeWithKey);
            foo.InjectedFieldValue.Should().Be("42");
            foo.InjectedFieldValueWithKey.Should().Be("43");
        }
    }
}