using UnityEngine;
using UnityEngine.Pool;
using ReflexPlus.Injectors;

namespace ReflexPlus.Core
{
    public class ContainerScope : MonoBehaviour
    {
        [SerializeField]
        private bool canInvokeOnSceneLoadedAction = true;

        public bool CanInvokeOnSceneLoadedAction
        {
            get => canInvokeOnSceneLoadedAction;
            set => canInvokeOnSceneLoadedAction = value;
        }

        private void Awake()
        {
            if (canInvokeOnSceneLoadedAction)
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