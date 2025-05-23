using NUnit.Framework;
using ReflexPlus.Core;

namespace ReflexPlusEditor.Tests
{
    internal class ParentOverrideScopeTests
    {
        [Test]
        public void ParentOverrideScope_WillOverrideAnyNewContainerParentWhileItsNotDisposed_ReturnsExpectedValues()
        {
            var parentOverride = new ContainerBuilder()
                .RegisterValue(95)
                .Build();

            using (new ParentOverrideScope(parentOverride))
            {
                var containerOneWithParentOverride = new ContainerBuilder().Build();
                Assert.That(containerOneWithParentOverride.Parent, Is.SameAs(parentOverride));
                Assert.That(containerOneWithParentOverride.Single<int>(), Is.EqualTo(95));

                var containerTwoWithParentOverride = new ContainerBuilder().Build();
                Assert.That(containerTwoWithParentOverride.Parent, Is.SameAs(parentOverride));
                Assert.That(containerTwoWithParentOverride.Single<int>(), Is.EqualTo(95));
            }

            var containerWithoutParentOverride = new ContainerBuilder().Build();
            Assert.That(containerWithoutParentOverride.Parent, Is.Null);
            Assert.That(containerWithoutParentOverride.HasBinding<int>(), Is.False);
        }
    }
}