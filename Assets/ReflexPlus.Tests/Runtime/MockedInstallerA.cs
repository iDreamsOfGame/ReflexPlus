using ReflexPlus.Core;
using UnityEngine;

namespace ReflexPlus.EditorTests
{
    public class MockedInstallerA : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue("A");
        }
    }
}