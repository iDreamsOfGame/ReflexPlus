using NUnit.Framework;
using ReflexPlus.Attributes;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable UnusedParameter.Local

namespace ReflexPlusEditor.Tests
{
    internal class ConstructorInjectAttributeTests
    {
        private class ClassWithReflexConstructorIdentifiedByMaxAmountOfArguments
        {
            public bool ConstructedWithEmptyConstructor { get; }

            public bool ConstructedWithNonEmptyConstructor { get; }

            public ClassWithReflexConstructorIdentifiedByMaxAmountOfArguments()
            {
                ConstructedWithEmptyConstructor = true;
            }

            public ClassWithReflexConstructorIdentifiedByMaxAmountOfArguments(object a, int b)
            {
                ConstructedWithNonEmptyConstructor = true;
            }
        }

        private class ClassWithReflexConstructorIdentifiedByReflexConstructorAttribute
        {
            public bool ConstructedWithEmptyConstructor { get; }

            public bool ConstructedWithNonEmptyConstructor { get; }

            [ConstructorInject]
            public ClassWithReflexConstructorIdentifiedByReflexConstructorAttribute()
            {
                ConstructedWithEmptyConstructor = true;
            }

            public ClassWithReflexConstructorIdentifiedByReflexConstructorAttribute(object a, int b)
            {
                ConstructedWithNonEmptyConstructor = true;
            }
        }

        private class ConstructorFoo
        {
            [ConstructorInject]
            private ConstructorFoo(object a, int b)
            {
            }
        }

        private class ConstructorFoo2
        {
            [ConstructorInject(true)]
            private ConstructorFoo2(object a, int b)
            {
            }
        }

        private class ConstructorFoo3
        {
            [ConstructorInject(null, 2)]
            private ConstructorFoo3(int a, int b)
            {
                A = a;
                B = b;
            }

            public int A { get; }

            public int B { get; }
        }

        [Test]
        public void ClassWithMultipleConstructors_WithoutAnyReflexConstructorAttribute_ReturnsCorrectValues()
        {
            var container = new ContainerBuilder()
                .RegisterValue(new object())
                .RegisterValue(42)
                .Build();

            var result = container.Construct<ClassWithReflexConstructorIdentifiedByMaxAmountOfArguments>();
            Assert.That(result.ConstructedWithEmptyConstructor, Is.False);
            Assert.That(result.ConstructedWithNonEmptyConstructor, Is.True);
        }

        [Test]
        public void ClassWithMultipleConstructors_WithOneDefiningReflexConstructorAttribute__ReturnsCorrectValues()
        {
            var container = new ContainerBuilder()
                .RegisterValue(new object())
                .RegisterValue(42)
                .Build();

            var result = container.Construct<ClassWithReflexConstructorIdentifiedByReflexConstructorAttribute>();
            Assert.That(result.ConstructedWithEmptyConstructor, Is.True);
            Assert.That(result.ConstructedWithNonEmptyConstructor, Is.False);
        }

        [Test]
        public void ConstructorInjector_ThrowsConstructorInjectorException()
        {
            Assert.Throws<ConstructorInjectorException>(() => new ContainerBuilder().Build().Construct<ConstructorFoo>());
        }

        [Test]
        public void ConstructorInjectorOptional_ReturnsNull()
        {
            var container = new ContainerBuilder()
                .Build();

            var foo = container.Construct<ConstructorFoo2>();
            Assert.That(foo, Is.Null);
        }

        [Test]
        public void ConstructorInjectorParameterWithKey_ReturnsNotNullAndCorrectValues()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .RegisterValue(43, 2)
                .Build();

            var foo = container.Construct<ConstructorFoo3>();
            Assert.That(foo, Is.Not.Null);
            Assert.That(foo.A, Is.EqualTo(42));
            Assert.That(foo.B, Is.EqualTo(43));
        }
    }
}