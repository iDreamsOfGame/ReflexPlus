using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Attributes;
using ReflexPlus.Core;

namespace ReflexPlus.EditModeTests
{
    public class CustomInjectAttributeTests
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
        public void CustomInheritorOfInjectAttribute_CanBeUsedToInjectFields()
        {
            using var container = new ContainerBuilder()
                .RegisterValue(42)
                .Build();

            var service = container.Construct<CustomInjectOnField>();
            service.GetNumber().Should().Be(42);
        }

        [Test]
        public void CustomInheritorOfInjectAttribute_CanBeUsedToInjectProperties()
        {
            using var container = new ContainerBuilder()
                .RegisterValue(42)
                .Build();

            var service = container.Construct<CustomInjectOnProperty>();
            service.GetNumber().Should().Be(42);
        }

        [Test]
        public void CustomInheritorOfInjectAttribute_CanBeUsedToInjectMethods()
        {
            using var container = new ContainerBuilder()
                .RegisterValue(42)
                .Build();

            var service = container.Construct<CustomInjectOnMethod>();
            service.GetNumber().Should().Be(42);
        }
    }
}