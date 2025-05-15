using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Attributes;
using ReflexPlus.Core;
using ReflexPlus.Exceptions;

// ReSharper disable ClassNeverInstantiated.Local
// ReSharper disable UnusedParameter.Local

namespace ReflexPlus.EditModeTests
{
    public class ConstructorInjectAttributeTests
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
        public void ClassWithMultipleConstructors_WithoutAnyReflexConstructorAttribute_ShouldBeConstructedUsing_ConstructorWithMostArguments()
        {
            var container = new ContainerBuilder()
                .RegisterValue(new object())
                .RegisterValue(42)
                .Build();

            var result = container.Construct<ClassWithReflexConstructorIdentifiedByMaxAmountOfArguments>();
            result.ConstructedWithEmptyConstructor.Should().BeFalse();
            result.ConstructedWithNonEmptyConstructor.Should().BeTrue();
        }

        [Test]
        public void ClassWithMultipleConstructors_WithOneDefiningReflexConstructorAttribute_ShouldBeConstructedUsing_ConstructorWithReflexConstructorAttribute()
        {
            var container = new ContainerBuilder()
                .RegisterValue(new object())
                .RegisterValue(42)
                .Build();

            var result = container.Construct<ClassWithReflexConstructorIdentifiedByReflexConstructorAttribute>();
            result.ConstructedWithEmptyConstructor.Should().BeTrue();
            result.ConstructedWithNonEmptyConstructor.Should().BeFalse();
        }

        [Test]
        public void ConstructorInjectorException()
        {
            var container = new ContainerBuilder()
                .Build();
            
            Assert.Throws<ConstructorInjectorException>(() => container.Construct<ConstructorFoo>());
        }

        [Test]
        public void ConstructorInjectorOptional()
        {
            var container = new ContainerBuilder()
                .Build();

            var foo = container.Construct<ConstructorFoo2>();
            Assert.Null(foo);
        }
        
        [Test]
        public void ConstructorInjectorParameterWithKey()
        {
            var container = new ContainerBuilder()
                .RegisterValue(42)
                .RegisterValue(43, 2)
                .Build();

            var foo = container.Construct<ConstructorFoo3>();
            Assert.NotNull(foo);
            Assert.AreEqual(foo.A, 42);
            Assert.AreEqual(foo.B, 43);
        }
    }
}