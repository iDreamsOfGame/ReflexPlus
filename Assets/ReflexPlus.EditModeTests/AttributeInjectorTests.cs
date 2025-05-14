using FluentAssertions;
using NUnit.Framework;
using Reflex.Attributes;
using Reflex.Core;

namespace Reflex.EditModeTests
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
        
        private class Foo2
        {
            [Inject]
            private readonly Foo fooInjectFieldValue;

            [Inject("other")]
            private readonly Foo fooInjectFieldValueWithKey;

            [Inject] 
            private readonly string injectedFieldValue;
            
            [Inject("other")]
            private readonly string injectedFieldValueWithKey;

            public Foo FooInjectFieldValue => fooInjectFieldValue;

            public Foo FooInjectFieldValueWithKey => fooInjectFieldValueWithKey;
            
            public string InjectedFieldValue => injectedFieldValue;
            
            public string InjectedFieldValueWithKey => injectedFieldValueWithKey;
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
                .RegisterValue(42)
                .RegisterValue("42")
                .RegisterValue("43", "other")
                .RegisterType<Foo>()
                .RegisterType(typeof(Foo), "other")
                .RegisterType<Foo2>()
                .RegisterType<Foo2>("instance2")
                .Build();
            
            var foo1 = container.Single<Foo2>();
            var foo2 = container.Single<Foo2>("instance2");
            Assert.NotNull(foo1);
            Assert.NotNull(foo2);
            Assert.AreNotEqual(foo1, foo2);
            foo2.InjectedFieldValue.Should().Be("42");
            foo2.InjectedFieldValueWithKey.Should().Be("43");
        }
    }
}