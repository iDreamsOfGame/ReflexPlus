using UnityEditor;
using UnityEngine;
using ReflexPlus.Core;
using ReflexPlusEditor.DebuggingWindow;
using UnityEditor.SceneManagement;

namespace ReflexPlusEditor
{
    internal static class ReflexPlusMenuItems
    {
        [MenuItem("Window/Analysis/Reflex+ Debugger %e")]
        private static void OpenReflexDebuggingWindow()
        {
            EditorWindow.GetWindow<ReflexPlusDebuggerWindow>(false, "Reflex+ Debugger", true);
        }

        [MenuItem("GameObject/Reflex+/ContainerScope")]
        private static void CreateReflexContainerScope()
        {
            var containerScope = new GameObject("ContainerScope").AddComponent<ContainerScope>();
            Selection.activeObject = containerScope.gameObject;
            EditorSceneManager.MarkSceneDirty(containerScope.gameObject.scene);
        }
    }
}