using UnityEditor;
using UnityEngine;

namespace ReflexPlusEditor.DebuggingWindow
{
    public static class Styles
    {
        private static GUIStyle richTextLabel;

        public static GUIStyle RichTextLabel => richTextLabel ??= new GUIStyle(EditorStyles.label) { richText = true };

        private static GUIStyle stackTrace;

        public static GUIStyle StackTrace => stackTrace ??= new GUIStyle("CN Message") { wordWrap = false };

        private static GUIStyle labelHorizontallyCentered;

        public static GUIStyle LabelHorizontallyCentered => labelHorizontallyCentered ??= new GUIStyle("Label") { alignment = TextAnchor.MiddleCenter };

        private static GUIStyle appToolbar;

        public static GUIStyle AppToolbar => appToolbar ??= new GUIStyle("AppToolbar");

        private static GUIStyle statusBarIcon;

        public static GUIStyle StatusBarIcon => statusBarIcon ??= new GUIStyle("StatusBarIcon");

        private static GUIStyle hyperlink;

        public static GUIStyle Hyperlink => hyperlink ??= new GUIStyle(EditorStyles.linkLabel) { wordWrap = false };
    }
}