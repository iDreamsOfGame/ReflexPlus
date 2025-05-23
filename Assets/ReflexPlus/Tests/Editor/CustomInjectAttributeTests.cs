using NUnit.Framework;
using ReflexPlus.Attributes;
using ReflexPlus.Core;

namespace ReflexPlusEditor.Tests
{
    internal class CustomInjectAttributeTests
    {
        private class CustomInjectAttribute : InjectAttribute
        {
        }

        public class CustomInjectOnField
        {
            [CustomInject]
            private int number;

            public int GetNumber() => number;
        }

        public class CustomInjectOnProperty
        {
            [CustomInject]
            private int Number { get; set; }

            public int GetNumber() => Number;
        }

        public class CustomInjectOnMethod
        {
            private int number;

            [CustomInject]
            private void Inject(int number)
            {
                this.number = number;
            }

            public int GetNumber() => number;
        }

        [Test]
        public void CustomInheritorOfInjectAttribute_CanBeUsedToInjectFields_ReturnsCorrectValue()
        {
            using var container = new ContainerBuilder()
                .RegisterValue(42)
                .Build();

            var service = container.Construct<CustomInjectOnField>();
            Assert.That(service.GetNumber(), Is.EqualTo(42));
        }

        [Test]
        public void CustomInheritorOfInjectAttribute_CanBeUsedToInjectProperties_ReturnsCorrectValue()
        {
            using var container = new ContainerBuilder()
                .RegisterValue(42)
                .Build();

            var service = container.Construct<CustomInjectOnProperty>();
            Assert.That(service.GetNumber(), Is.EqualTo(42));
        }

        [Test]
        public void CustomInheritorOfInjectAttribute_CanBeUsedToInjectMethods_ReturnsCorrectValue()
        {
            using var container = new ContainerBuilder()
                .RegisterValue(42)
                .Build();

            var service = container.Construct<CustomInjectOnMethod>();
            Assert.That(service.GetNumber(), Is.EqualTo(42));
        }
    }
}