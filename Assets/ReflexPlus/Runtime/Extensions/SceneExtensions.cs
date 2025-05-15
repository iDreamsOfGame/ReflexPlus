using ReflexPlus.Core;
using ReflexPlus.Injectors;
using UnityEngine.SceneManagement;

namespace ReflexPlus.Extensions
{
    public static class SceneExtensions
    {
        public static Container GetSceneContainer(this Scene scene)
        {
            return UnityInjector.ContainersPerScene[scene];
        }
    }
}