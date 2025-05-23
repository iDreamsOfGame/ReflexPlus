using System.Collections;
using System.Linq;
using NUnit.Framework;
using ReflexPlus.Extensions;
using ReflexPlus.Injectors;
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
        public void ExecutionOrderOf_PreInstantiated_InjectedObject_ReturnsCorrectString()
        {
            var injectedObject = Object.FindObjectsOfType<InjectedGameObject>().Single();
            Assert.That(string.Join(",", injectedObject.ExecutionOrder), Is.EqualTo("Inject,Awake,Start"));
        }

        [UnityTest]
        public IEnumerator ExecutionOrderOf_RuntimeInstantiated_InjectedObject_ReturnsCorrectString()
        {
            var prefab = new GameObject("Prefab").AddComponent<InjectedGameObject>();
            prefab.gameObject.SetActive(false);
            var injectedObject = Object.Instantiate(prefab);
            GameObjectInjector.InjectRecursive(injectedObject.gameObject, injectedObject.gameObject.scene.GetSceneContainer());
            injectedObject.gameObject.SetActive(true);
            yield return WaitFrame(); // Wait until Start is called, takes one frame
            Assert.That(string.Join(",", injectedObject.ExecutionOrder), Is.EqualTo("Inject,Awake,Start"));
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