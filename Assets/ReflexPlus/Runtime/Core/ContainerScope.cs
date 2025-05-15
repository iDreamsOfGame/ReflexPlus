using UnityEngine;
using UnityEngine.Pool;
using ReflexPlus.Injectors;

namespace ReflexPlus.Core
{
    public class ContainerScope : MonoBehaviour
    {
        private void Awake()
        {
            UnityInjector.OnSceneLoaded.Invoke(gameObject.scene, this);
        }

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            using (ListPool<IInstaller>.Get(out var installers))
            {
                GetComponentsInChildren(installers);

                foreach (var installer in installers)
                {
                    installer.InstallBindings(containerBuilder);
                }
            }
        }
    }
}