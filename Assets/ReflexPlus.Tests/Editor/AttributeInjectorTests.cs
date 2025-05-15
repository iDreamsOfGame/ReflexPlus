using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Attributes;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

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
        
        private class FieldFooInner
        {
            [Inject(true)]
            private readonly byte injectFieldOptional;
        }

        private class FieldFooInner2
        {
            [Inject]
            private readonly byte injectField;
        }

        private class FieldFoo
        {
            [Inject]
            private readonly string injectedFieldValue;

            [Inject("other")]
            private readonly string injectedFieldValueWithKey;

            [Inject]
            private readonly FieldFooInner injectedFieldType;

            [Inject("other")]
            private readonly FieldFooInner injectedFieldTypeWithKey;

            public string InjectedFieldValue => injectedFieldValue;

            public string InjectedFieldValueWithKey => injectedFieldValueWithKey;

            public FieldFooInner InjectedFieldType => injectedFieldType;

            public FieldFooInner InjectedFieldTypeWithKey => injectedFieldTypeWithKey;
        }

        private class FieldFoo2
        {
            [Inject]
            private FieldFooInner2 injectedFieldType;
        }
        
        private class PropertyFooInner
        {
            [Inject(true)]
            public byte InjectPropertyOptional { get; private set; }
        }

        private class PropertyFooInner2
        {
            [Inject]
            public byte InjectProperty { get; private set; }
        }

        private class PropertyFoo
        {
            [Inject]
            public string InjectedPropertyValue { get; private set; }
            
            [Inject(2)]
            public string InjectedPropertyValueWithKey { get; private set; }

            [Inject]
            public PropertyFooInner InjectedPropertyType { get; private set; }

            [Inject(2)]
            public PropertyFooInner InjectedPropertyTypeWithKey { get; private set; }
        }
        
        private class PropertyFoo2
        {
            [Inject]
            public PropertyFooInner2 InjectedPropertyType { get; private set; }
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
                .RegisterType<FieldFooInner>()
                .RegisterType(typeof(FieldFooInner), "other")
                .RegisterType<FieldFoo>()
                .Build();

            var foo = container.Single<FieldFoo>();
            Assert.NotNull(foo);
            Assert.AreNotEqual(foo.InjectedFieldType, foo.InjectedFieldTypeWithKey);
            foo.InjectedFieldValue.Should().Be("42");
            foo.InjectedFieldValueWithKey.Should().Be("43");
        }

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnFieldsMarkedByInjectAttributeWithoutOptional()
        {
            var container = new ContainerBuilder()
                .RegisterType<FieldFoo2>()
                .Build();
            
            Assert.Throws<FieldInjectorException>(() => container.Single<FieldFoo2>());
        }

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnPropertiesMarkedByInjectAttributeWithKey()
        {
            var container = new ContainerBuilder()
                .RegisterValue("42")
                .RegisterValue("43", 2)
                .RegisterType<PropertyFooInner>()
                .RegisterType(typeof(PropertyFooInner), 2)
                .RegisterType<PropertyFoo>()
                .Build();
            
            var foo = container.Single<PropertyFoo>();
            Assert.NotNull(foo);
            Assert.AreNotEqual(foo.InjectedPropertyType, foo.InjectedPropertyTypeWithKey);
            foo.InjectedPropertyValue.Should().Be("42");
            foo.InjectedPropertyValueWithKey.Should().Be("43");
        }

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnPropertiesMarkedByInjectAttributeWithoutOptional()
        {
            var container = new ContainerBuilder()
                .RegisterType<PropertyFoo2>()
                .Build();
            
            Assert.Throws<PropertyInjectorException>(() => container.Single<PropertyFoo2>());
        }
    }
}