using ReflexPlus.Core;
using UnityEngine;

namespace ReflexPlus.PlayModeTests
{
    public class MockedInstallerC : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue("C");
        }
    }
}