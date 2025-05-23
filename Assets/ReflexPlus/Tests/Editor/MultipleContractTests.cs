using System.Linq;
using NUnit.Framework;
using ReflexPlus.Core;

namespace ReflexPlusEditor.Tests
{
    internal class MultipleContractTests
    {
        private interface IManager
        {
        }

        private interface IBundleManager : IManager
        {
        }

        private interface IPrefabManager : IManager
        {
        }

        private class BundleManager : IBundleManager
        {
        }

        private class PrefabManager : IPrefabManager
        {
        }

        [Test]
        public void SingletonWithMultipleContractsCanBeResolved_ReturnsExpectedLengthAndContainsExpectedInstances()
        {
            var container = new ContainerBuilder()
                .RegisterType(typeof(BundleManager), new[] { typeof(IBundleManager), typeof(IManager) })
                .RegisterType(typeof(PrefabManager), new[] { typeof(IPrefabManager), typeof(IManager) })
                .Build();

            var bundleManager = container.Single<IBundleManager>();
            var prefabManager = container.Single<IPrefabManager>();

            var managers = container.All<IManager>().ToArray();
            Assert.That(managers.Length, Is.EqualTo(2));
            Assert.That(managers, Is.EquivalentTo(new IManager[] { bundleManager, prefabManager }));
        }
    }
}