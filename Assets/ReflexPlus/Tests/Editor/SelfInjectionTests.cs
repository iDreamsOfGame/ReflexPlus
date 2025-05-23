using NUnit.Framework;
using ReflexPlus.Core;

namespace ReflexPlusEditor.Tests
{
    internal class SelfInjectionTests
    {
        [Test]
        public void Container_ShouldBeAbleToResolveItself_ReturnExpectedInstance()
        {
            var container = new ContainerBuilder().Build();
            Assert.That(container.Single<Container>(), Is.SameAs(container));
        }
    }
}