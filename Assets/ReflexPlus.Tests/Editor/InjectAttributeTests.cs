using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Attributes;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

// ReSharper disable ClassNeverInstantiated.Local

namespace ReflexPlus.EditModeTests
{
    internal class InjectAttributeTests
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
            private readonly byte injectedFieldOptional;
        }

        private class FieldFooInner2
        {
            [Inject]
            private readonly byte injectedField;
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
            public byte InjectedPropertyOptional { get; private set; }
        }

        private class PropertyFooInner2
        {
            [Inject]
            public byte InjectedProperty { get; private set; }
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
        
        private class MethodFooInner
        {
            [Inject(true)]
            private byte injectedFieldOptional;
            
            [Inject(true)]
            public byte InjectedPropertyOptional { get; private set; }
        }

        private class MethodFooInner2
        {
            [Inject]
            private byte injectedField;
            
            [Inject]
            public byte InjectedProperty { get; private set; }
        }

        private class MethodFoo
        {
            public string InjectedValue1 { get; private set; }

            public string InjectedValue2 { get; private set; }

            public string InjectedValue3 { get; private set; }

            public MethodFooInner InjectedType1 { get; private set; }

            public MethodFooInner InjectedType2 { get; private set; }

            public MethodFooInner InjectedType3 { get; private set; }

            [Inject]
            private void Inject1(string value1, MethodFooInner value2)
            {
                InjectedValue1 = value1;
                InjectedType1 = value2;
            }
            
            [Inject(null, 2, null, 2)]
            private void Inject2(string value1, string value2, MethodFooInner value3, MethodFooInner value4)
            {
                InjectedValue2 = value1;
                InjectedValue3 = value2;
                InjectedType2 = value3;
                InjectedType3 = value4;
            }
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
            Assert.AreNotSame(foo.InjectedFieldType, foo.InjectedFieldTypeWithKey);
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
            Assert.AreNotSame(foo.InjectedPropertyType, foo.InjectedPropertyTypeWithKey);
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

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnMethodsMarkedByInjectAttributeWithKey()
        {
            var container = new ContainerBuilder()
                .RegisterValue("42")
                .RegisterValue("43", 2)
                .RegisterType<MethodFooInner>()
                .RegisterType(typeof(MethodFooInner), 2)
                .RegisterType<MethodFoo>()
                .Build();
            
            var foo = container.Single<MethodFoo>();
            Assert.NotNull(foo);
            Assert.AreEqual(foo.InjectedValue1, "42");
            Assert.AreEqual(foo.InjectedValue2, "42");
            Assert.AreEqual(foo.InjectedValue3, "43");
            Assert.NotNull(foo.InjectedType1);
            Assert.NotNull(foo.InjectedType2);
            Assert.NotNull(foo.InjectedType3);
            Assert.AreSame(foo.InjectedType1, foo.InjectedType2);
            Assert.AreNotSame(foo.InjectedType1, foo.InjectedType3);
            Assert.AreNotSame(foo.InjectedType2, foo.InjectedType3);
        }
    }
}