using System;
using ReflexPlus.Extensions;
using UnityEngine;
using UnityEngine.Serialization;

namespace ReflexPlus.Injectors
{
    [DefaultExecutionOrder(int.MinValue + 1000)]
    internal sealed class GameObjectSelfInjector : MonoBehaviour
    {
        [FormerlySerializedAs("_injectionStrategy")]
        [SerializeField]
        private InjectionStrategy injectionStrategy = InjectionStrategy.Recursive;

        private void Awake()
        {
            var sceneContainer = gameObject.scene.GetSceneContainer();

            switch (injectionStrategy)
            {
                case InjectionStrategy.Single:
                    GameObjectInjector.InjectSingle(gameObject, sceneContainer);
                    break;

                case InjectionStrategy.Object:
                    GameObjectInjector.InjectObject(gameObject, sceneContainer);
                    break;

                case InjectionStrategy.Recursive:
                    GameObjectInjector.InjectRecursive(gameObject, sceneContainer);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(injectionStrategy.ToString());
            }
        }

        private enum InjectionStrategy
        {
            Single,

            Object,

            Recursive
        }
    }
}