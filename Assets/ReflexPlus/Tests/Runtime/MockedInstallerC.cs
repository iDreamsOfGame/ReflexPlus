using ReflexPlus.Core;
using UnityEngine;

namespace ReflexPlus.EditorTests
{
    public class MockedInstallerC : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterValue("C");
        }
    }
}