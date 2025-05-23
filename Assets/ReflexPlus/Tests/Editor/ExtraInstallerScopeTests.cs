using NUnit.Framework;
using ReflexPlus.Core;

namespace ReflexPlusEditor.Tests
{
    internal class ExtraInstallerScopeTests
    {
        [Test]
        public void ExtraInstallerScope_WillInstallOnAnyNewContainerWhileItsNotDisposed_ReturnsCorrectValues()
        {
            using (new ExtraInstallerScope(b => b.RegisterValue(95)))
            {
                var containerOneWithExtraInstaller = new ContainerBuilder().Build();
                Assert.That(containerOneWithExtraInstaller.Single<int>(), Is.EqualTo(95));

                var containerTwoWithExtraInstaller = new ContainerBuilder().Build();
                Assert.That(containerTwoWithExtraInstaller.Single<int>(), Is.EqualTo(95));
            }

            var containerWithoutExtraInstaller = new ContainerBuilder().Build();
            Assert.That(containerWithoutExtraInstaller.HasBinding<int>(), Is.False);
        }
    }
}