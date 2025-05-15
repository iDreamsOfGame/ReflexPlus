using ReflexPlus.Core;
using ReflexPlus.Sample.Application;
using UnityEngine;

namespace ReflexPlus.Sample.Infrastructure
{
    internal class ReflexSampleSceneInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private CollectorConfigurationModel _collectorConfigurationModel;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            InstallInput(containerBuilder, useMouse: false);
            containerBuilder.RegisterValue(_collectorConfigurationModel);
            containerBuilder.RegisterType(typeof(CollectionStoragePrefs), new[] { typeof(ICollectionStorage) });
        }

        private static void InstallInput(ContainerBuilder containerBuilder, bool useMouse)
        {
            var implementation = useMouse ? typeof(CollectorInputMouse) : typeof(CollectorInputKeyboard);
            containerBuilder.RegisterType(implementation, new[] { typeof(ICollectorInput) });
        }
    }
}