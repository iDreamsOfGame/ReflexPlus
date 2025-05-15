using ReflexPlus.Core;
using ReflexPlus.Sample.Application;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReflexPlus.Sample.Infrastructure
{
    internal class ReflexSampleSceneInstaller : MonoBehaviour, IInstaller
    {
        [FormerlySerializedAs("_collectorConfigurationModel")]
        [SerializeField]
        private CollectorConfigurationModel collectorConfigurationModel;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            InstallInput(containerBuilder, useMouse: false);
            containerBuilder.RegisterValue(collectorConfigurationModel);
            containerBuilder.RegisterType(typeof(CollectionStoragePrefs), new[] { typeof(ICollectionStorage) });
        }

        private static void InstallInput(ContainerBuilder containerBuilder, bool useMouse)
        {
            var implementation = useMouse ? typeof(CollectorInputMouse) : typeof(CollectorInputKeyboard);
            containerBuilder.RegisterType(implementation, new[] { typeof(ICollectorInput) });
        }
    }
}