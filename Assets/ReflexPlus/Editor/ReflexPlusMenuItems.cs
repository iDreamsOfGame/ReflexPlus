using System.IO;
using UnityEditor;
using UnityEngine;
using ReflexPlus;
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

        [MenuItem("Assets/Create/Reflex+/Settings")]
        private static void CreateReflexSettings()
        {
            var directory = UnityEditorUtility.GetSelectedPathInProjectWindow();
            var desiredAssetPath = Path.Combine(directory, "ReflexPlusSettings.asset");
            UnityEditorUtility.CreateScriptableObject<ReflexPlusSettings>(desiredAssetPath);
        }

        [MenuItem("Assets/Create/Reflex+/RootScope")]
        private static void CreateReflexRootScope()
        {
            var directory = UnityEditorUtility.GetSelectedPathInProjectWindow();
            var desiredAssetPath = Path.Combine(directory, "RootScope.prefab");

            UnityEditorUtility.CreatePrefab(desiredAssetPath, Edit);
            return;

            void Edit(GameObject prefab)
            {
                prefab.AddComponent<ContainerScope>();
            }
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