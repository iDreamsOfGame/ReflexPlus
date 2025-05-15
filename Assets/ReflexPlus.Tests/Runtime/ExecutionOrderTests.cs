using System.Collections;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using ReflexPlus.Extensions;
using ReflexPlus.Injectors;
using ReflexPlus.Tests;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace ReflexPlus.EditorTests
{
    public class ExecutionOrderTests
    {
        [UnitySetUp]
        public IEnumerator Setup()
        {
            yield return SceneManager.LoadSceneAsync("ExecutionOrderTestsScene", LoadSceneMode.Single);
            yield return WaitFrame(); // Wait until Start is called, takes one frame
        }

        [Test]
        public void ExecutionOrderOf_PreInstantiated_InjectedObject_ShouldBe_InjectAwakeStart()
        {
            var injectedObject = Object.FindObjectsOfType<InjectedGameObject>().Single();
            string.Join(",", injectedObject.ExecutionOrder).Should().Be("Inject,Awake,Start");
        }

        [UnityTest]
        public IEnumerator ExecutionOrderOf_RuntimeInstantiated_InjectedObject_ShouldBe_InjectAwakeStart()
        {
            var prefab = new GameObject("Prefab").AddComponent<InjectedGameObject>();
            prefab.gameObject.SetActive(false);
            var injectedObject = Object.Instantiate(prefab);
            GameObjectInjector.InjectRecursive(injectedObject.gameObject, injectedObject.gameObject.scene.GetSceneContainer());
            injectedObject.gameObject.SetActive(true);
            yield return WaitFrame(); // Wait until Start is called, takes one frame
            string.Join(",", injectedObject.ExecutionOrder).Should().Be("Inject,Awake,Start");
        }

        /// <summary>
        /// yield return new WaitForEndOfFrame() does not work when running tests on cli, it hangs
        /// See https://docs.unity3d.com/2022.3/Documentation/Manual/CLIBatchmodeCoroutines.html
        /// </summary>
        /// <returns></returns>
        private static IEnumerator WaitFrame()
        {
            var current = Time.frameCount;

            while (current == Time.frameCount)
            {
                yield return null;
            }
        }
    }
}