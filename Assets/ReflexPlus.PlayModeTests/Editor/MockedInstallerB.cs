using ReflexPlus.Core;
using UnityEngine;

namespace ReflexPlus.PlayModeTests
{
    public class MockedInstallerB : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue("B");
        }
    }
}