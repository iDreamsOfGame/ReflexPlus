using NUnit.Framework;
using ReflexPlus.Attributes;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

// ReSharper disable ClassNeverInstantiated.Local

namespace ReflexPlusEditor.Tests
{
    internal class InjectAttributeTests
    {
        private interface IFoo
        {
            int InjectedFieldValue { get; }

            int InjectedPropertyValue { get; }

            int InjectedMethodValue { get; }
        }
        
        private class Foo : IFoo
        {
            [Inject(true)]
            private readonly int injectedFieldValue;

            [Inject(true)]
            public int InjectedPropertyValue { get; private set; }

            public int InjectedFieldValue => injectedFieldValue;

            public int InjectedMethodValue { get; private set; }

            [Inject(true)]
            private void Inject(int value)
            {
                InjectedMethodValue = value;
            }
        }
        
        private class Foo2
        {
            [Inject]
            private IFoo injectedFieldValue;

            [Inject]
            public IFoo InjectedPropertyValue { get; private set; }

            public IFoo InjectedFieldValue => injectedFieldValue;

            public IFoo InjectedMethodValue { get; private set; }

            [Inject]
            private void Inject(IFoo value)
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
        public void AddSingleton_ShouldRunAttributeInjectionOnFieldsPropertiesAndMethodsMarkedByInjectAttribute_ReturnsExpectedValues()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .RegisterType(typeof(Foo))
                .Build();

            var foo = container.Single<Foo>();
            Assert.That(foo.InjectedFieldValue, Is.EqualTo(42));
            Assert.That(foo.InjectedPropertyValue, Is.EqualTo(42));
            Assert.That(foo.InjectedMethodValue, Is.EqualTo(42));
        }

        [Test]
        public void AddSingleton_ShouldRunInterfaceAttributeInjectionOnFieldsPropertiesAndMethodsMarkedByInjectAttribute_ReturnsSameInstance()
        {
            var container = new ContainerBuilder()
                .RegisterType<Foo, IFoo>()
                .RegisterType<Foo2>()
                .Build();

            var foo2 = container.Single<Foo2>();
            Assert.That(foo2, Is.Not.Null);
            Assert.That(foo2.InjectedFieldValue, Is.Not.Null);
            Assert.That(foo2.InjectedPropertyValue, Is.Not.Null);
            Assert.That(foo2.InjectedMethodValue, Is.Not.Null);
            Assert.That(foo2.InjectedFieldValue, Is.SameAs(foo2.InjectedPropertyValue));
            Assert.That(foo2.InjectedFieldValue, Is.SameAs(foo2.InjectedMethodValue));
        }

        [Test]
        public void AddSingleton_BuildContainerMoreThanOnce_ReturnsSameInstance()
        {
            var builder = new ContainerBuilder();
            var container = builder
                .RegisterValue(42)
                .RegisterType<Foo, IFoo>()
                .Build();
            
            var foo = container.Single<IFoo>();
            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.InjectedFieldValue, Is.EqualTo(42));

            container = builder.SetParent(container)
                .RegisterType<Foo2>()
                .Build();
            
            var foo2 = container.Single<Foo2>();
            Assert.That(foo2, Is.Not.Null);
            Assert.That(foo2.InjectedFieldValue, Is.SameAs(foo));
        }

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnFieldsMarkedByInjectAttributeWithKey_ReturnsExpectedValues()
        {
            var container = new ContainerBuilder()
                .RegisterValue("42")
                .RegisterValue("43", "other")
                .RegisterType<FieldFooInner>()
                .RegisterType(typeof(FieldFooInner), "other")
                .RegisterType<FieldFoo>()
                .Build();

            var foo = container.Single<FieldFoo>();
            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.InjectedFieldType, Is.Not.SameAs(foo.InjectedFieldTypeWithKey));
            Assert.That(foo.InjectedFieldValue, Is.EqualTo("42"));
            Assert.That(foo.InjectedFieldValueWithKey, Is.EqualTo("43"));
        }

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnFieldsMarkedByInjectAttributeWithoutOptional_ThrowsFieldInjectorException()
        {
            var container = new ContainerBuilder()
                .RegisterType<FieldFoo2>()
                .Build();
            
            Assert.Throws<FieldInjectorException>(() => container.Single<FieldFoo2>());
        }

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnPropertiesMarkedByInjectAttributeWithKey_ReturnsExpectedValues()
        {
            var container = new ContainerBuilder()
                .RegisterValue("42")
                .RegisterValue("43", 2)
                .RegisterType<PropertyFooInner>()
                .RegisterType(typeof(PropertyFooInner), 2)
                .RegisterType<PropertyFoo>()
                .Build();
            
            var foo = container.Single<PropertyFoo>();
            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.InjectedPropertyType, Is.Not.SameAs(foo.InjectedPropertyTypeWithKey));
            Assert.That(foo.InjectedPropertyValue, Is.EqualTo("42"));
            Assert.That(foo.InjectedPropertyValueWithKey, Is.EqualTo("43"));
        }

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnPropertiesMarkedByInjectAttributeWithoutOptional_ThrowsPropertyInjectorException()
        {
            var container = new ContainerBuilder()
                .RegisterType<PropertyFoo2>()
                .Build();
            
            Assert.Throws<PropertyInjectorException>(() => container.Single<PropertyFoo2>());
        }

        [Test]
        public void AddSingleton_ShouldRunAttributeInjectionOnMethodsMarkedByInjectAttributeWithKey_ReturnsExpectedValuesAndSameOrNotSameInstance()
        {
            var container = new ContainerBuilder()
                .RegisterValue("42")
                .RegisterValue("43", 2)
                .RegisterType<MethodFooInner>()
                .RegisterType(typeof(MethodFooInner), 2)
                .RegisterType<MethodFoo>()
                .Build();
            
            var foo = container.Single<MethodFoo>();
            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.InjectedValue1, Is.EqualTo("42"));
            Assert.That(foo.InjectedValue2, Is.EqualTo("42"));
            Assert.That(foo.InjectedValue3, Is.EqualTo("43"));
            Assert.That(foo.InjectedType1, Is.Not.Null);
            Assert.That(foo.InjectedType2, Is.Not.Null);
            Assert.That(foo.InjectedType3, Is.Not.Null);
            Assert.That(foo.InjectedType1, Is.SameAs(foo.InjectedType2));
            Assert.That(foo.InjectedType1, Is.Not.SameAs(foo.InjectedType3));
            Assert.That(foo.InjectedType2, Is.Not.SameAs(foo.InjectedType3));
        }
        
        [Test]
        public void AddTransient_ShouldRunAttributeInjectionOnFieldsPropertiesAndMethodsMarkedWithInjectAttribute_ReturnsExpectedValues()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .RegisterType(typeof(Foo))
                .Build();

            var foo = container.Single<Foo>();
            Assert.That(foo.InjectedFieldValue, Is.EqualTo(42));
            Assert.That(foo.InjectedPropertyValue, Is.EqualTo(42));
            Assert.That(foo.InjectedMethodValue, Is.EqualTo(42));
        }
    }
}