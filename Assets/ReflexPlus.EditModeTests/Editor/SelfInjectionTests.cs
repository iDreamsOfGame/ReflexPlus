using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Core;

namespace ReflexPlus.EditModeTests
{
    internal class SelfInjectionTests
    {
        [Test]
        public void Container_ShouldBeAbleToResolveItself()
        {
            var container = new ContainerBuilder().Build();
            container.Single<Container>().Should().Be(container);
        }
    }
}