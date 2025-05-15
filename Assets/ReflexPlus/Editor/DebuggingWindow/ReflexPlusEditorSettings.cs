using UnityEditor;

namespace ReflexPlusEditor.DebuggingWindow
{
    public static class ReflexPlusEditorSettings
    {
        public static bool ShowInternalBindings
        {
            get => EditorPrefs.GetBool("ShowInternalBindings", defaultValue: true);
            set => EditorPrefs.SetBool("ShowInternalBindings", value);
        }

        public static bool ShowInheritedBindings
        {
            get => EditorPrefs.GetBool("ShowInheritedBindings", defaultValue: true);
            set => EditorPrefs.SetBool("ShowInheritedBindings", value);
        }
    }
}