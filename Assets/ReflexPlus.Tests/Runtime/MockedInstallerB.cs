using ReflexPlus.Core;
using UnityEngine;

namespace ReflexPlus.EditorTests
{
    public class MockedInstallerB : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue("B");
        }
    }
}