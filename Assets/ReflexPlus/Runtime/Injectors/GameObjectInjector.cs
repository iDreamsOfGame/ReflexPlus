using System.Collections.Generic;
using ReflexPlus.Core;
using UnityEngine;
using UnityEngine.Pool;

// ReSharper disable ForCanBeConvertedToForeach

namespace ReflexPlus.Injectors
{
    public static class GameObjectInjector
    {
        public static void InjectSingle(GameObject gameObject, Container container)
        {
            if (gameObject.TryGetComponent<MonoBehaviour>(out var monoBehaviour))
            {
                AttributeInjector.InjectInto(monoBehaviour, null, container);
            }
        }

        public static void InjectObject(GameObject gameObject, Container container)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            gameObject.GetComponents(monoBehaviours);

            for (var i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];
                if (monoBehaviour)
                    AttributeInjector.InjectInto(monoBehaviour, null, container);
            }
        }

        public static void InjectRecursive(GameObject gameObject, Container container)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);
            gameObject.GetComponentsInChildren(true, monoBehaviours);

            for (var i = 0; i < monoBehaviours.Count; i++)
            {
                var monoBehaviour = monoBehaviours[i];
                if (monoBehaviour)
                    AttributeInjector.InjectInto(monoBehaviour, null, container);
            }
        }

        public static void InjectRecursiveMany(List<GameObject> gameObject, Container container)
        {
            using var pooledObject = ListPool<MonoBehaviour>.Get(out var monoBehaviours);

            for (var i = 0; i < gameObject.Count; i++)
            {
                gameObject[i].GetComponentsInChildren(true, monoBehaviours);

                for (var j = 0; j < monoBehaviours.Count; j++)
                {
                    var monoBehaviour = monoBehaviours[j];
                    if (monoBehaviour)
                        AttributeInjector.InjectInto(monoBehaviour, null, container);
                }
            }
        }
    }
}