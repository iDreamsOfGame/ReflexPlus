using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReflexPlus.Extensions;
using ReflexPlus.Core;
using ReflexPlus.Injectors;
using ReflexPlus;
using ReflexPlus.Resolvers;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace ReflexPlusEditor.DebuggingWindow
{
    public class ReflexPlusDebuggerWindow : EditorWindow
    {
        private const string ContainerIcon = "PreMatCylinder"; // d_PrefabModel On Icon, "PreMatCylinder"

        private const string ResolverIcon = "d_NetworkAnimator Icon"; // "d_eyeDropper.Large", "AnimatorStateTransition Icon", "RelativeJoint2D Icon"

        private const string InstanceIcon = "d_Prefab Icon"; // "d_Prefab Icon", "d_Prefab On Icon"

        [NonSerialized]
        private bool hasInitialized;

        [FormerlySerializedAs("_treeViewState")]
        [SerializeField]
        private TreeViewState treeViewState; // Serialized in the window layout file so it survives assembly reloading

        [FormerlySerializedAs("_multiColumnHeaderState")]
        [SerializeField]
        private MultiColumnHeaderState multiColumnHeaderState;

        private int id = -1;

        private SearchField searchField;

        private Vector2 bindingStackTraceScrollPosition;

        private MultiColumnTreeView TreeView { get; set; }

        private Rect SearchBarRect => new Rect(20f, 10f, position.width - 40f, 20f);

        private Rect MultiColumnTreeViewRect => new Rect(20, 30, position.width - 40, position.height - 200);

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            try
            {
                await Task.Yield();
                Refresh();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async void OnSceneUnloaded(Scene scene)
        {
            try
            {
                await Task.Yield();
                Refresh();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async void OnPlayModeStateChanged(PlayModeStateChange playModeStateChange)
        {
            try
            {
                await Task.Yield();
                Refresh();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private void InitIfNeeded()
        {
            if (!hasInitialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)
                treeViewState ??= new TreeViewState();
                var firstInit = multiColumnHeaderState == null;
                var headerState = MultiColumnTreeView.CreateDefaultMultiColumnHeaderState();
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(multiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(multiColumnHeaderState, headerState);
                multiColumnHeaderState = headerState;

                var multiColumnHeader = new MultiColumnHeader(headerState)
                {
                    canSort = false,
                    height = MultiColumnHeader.DefaultGUI.minimumHeight
                };

                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                var treeModel = new TreeModel<MyTreeElement>(GetData());

                TreeView = new MultiColumnTreeView(treeViewState, multiColumnHeader, treeModel);
                TreeView.ExpandAll();

                searchField = new SearchField();
                searchField.downOrUpArrowKeyPressed += TreeView.SetFocusAndEnsureSelectedItem;

                hasInitialized = true;
            }
        }

        private static List<(IResolver, Type[])> BuildMatrix(Container container)
        {
            var resolvers = container.ResolversByContract.Values
                .SelectMany(r => r)
                .ToHashSet();

            if (ReflexPlusEditorSettings.ShowInternalBindings == false)
            {
                var registrationId = new RegistrationId(typeof(Container));
                var containerResolver = container.ResolversByContract[registrationId].Single();
                resolvers.Remove(containerResolver);
            }

            if (ReflexPlusEditorSettings.ShowInheritedBindings == false && container.Parent != null)
            {
                var parentResolvers = container.Parent.ResolversByContract.Values
                    .SelectMany(r => r)
                    .Distinct();

                foreach (var parentResolver in parentResolvers)
                {
                    resolvers.Remove(parentResolver);
                }
            }

            return resolvers.Select(resolver => (resolver, GetContracts(resolver, container))).ToList();
        }

        private static Type[] GetContracts(IResolver resolver, Container container)
        {
            var result = new List<Type>();

            foreach (var (registrationId, resolvers) in container.ResolversByContract)
            {
                if (resolvers.Contains(resolver))
                {
                    result.Add(registrationId.Type);
                }
            }

            return result.ToArray();
        }

        private void BuildDataRecursively(MyTreeElement parent, Container container)
        {
            if (container == null)
            {
                return;
            }

            var containerTreeElement = new MyTreeElement(container.Name, parent.Depth + 1, ++id, ContainerIcon, () => string.Empty, Array.Empty<string>(), string.Empty, container.GetDebugProperties().BuildCallsite, kind: string.Empty);
            parent.Children.Add(containerTreeElement);
            containerTreeElement.Parent = parent;

            foreach (var pair in BuildMatrix(container))
            {
                var resolverTreeElement = new MyTreeElement(
                    string.Join(", ", pair.Item2.Select(x => x.GetName())), // In this case Name is not used for rendering, but for searching
                    containerTreeElement.Depth + 1,
                    ++id,
                    ResolverIcon,
                    () => pair.Item1.GetDebugProperties().Resolutions.ToString(),
                    pair.Item2.Select(x => x.GetName()).OrderBy(x => x).ToArray(),
                    pair.Item1.Lifetime.ToString(),
                    pair.Item1.GetDebugProperties().BindingCallsite,
                    kind: pair.Item1.GetType().Name.Replace("Singleton", string.Empty).Replace("Transient", string.Empty).Replace("Scoped", string.Empty).Replace("Resolver", string.Empty)
                );

                foreach (var (instance, callsite) in pair.Item1.GetDebugProperties().Instances.Where(tuple => tuple.Item1.IsAlive).Select(tuple => (tuple.Item1.Target, tuple.Item2)))
                {
                    var instanceTreeElement = new MyTreeElement(
                        $"{instance.GetType().GetName()} <color=#3D99ED>({SHA1.ShortHash(instance.GetHashCode())})</color>",
                        resolverTreeElement.Depth + 1,
                        ++id,
                        InstanceIcon,
                        () => string.Empty,
                        Array.Empty<string>(),
                        string.Empty,
                        callsite,
                        string.Empty
                    );

                    instanceTreeElement.SetParent(resolverTreeElement);
                }

                resolverTreeElement.SetParent(containerTreeElement);
            }

            foreach (var scopedContainer in container.Children)
            {
                BuildDataRecursively(containerTreeElement, scopedContainer);
            }
        }

        private IList<MyTreeElement> GetData()
        {
            var root = new MyTreeElement("Root", -1, ++id, ContainerIcon, () => string.Empty, Array.Empty<string>(), string.Empty, null, string.Empty);
            BuildDataRecursively(root, UnityInjector.RootContainer);

            var list = new List<MyTreeElement>();
            TreeElementUtility.TreeToList(root, list);
            return list;
        }

        private void OnGUI()
        {
            Repaint();
            InitIfNeeded();

            if (UnityScriptingDefineSymbols.IsDefined("REFLEX_PLUS_DEBUG"))
            {
                PresentDebuggerEnabled();
            }
            else
            {
                PresentDebuggerDisabled();
            }

            GUILayout.FlexibleSpace();
            PresentStatusBar();
        }

        private void Refresh(PlayModeStateChange _ = default)
        {
            hasInitialized = false;
            InitIfNeeded();
        }

        private void SearchBar(Rect rect)
        {
            TreeView.searchString = searchField.OnGUI(rect, TreeView.searchString);
            GUILayoutUtility.GetRect(rect.width, rect.height);
        }

        private void DoTreeView(Rect rect)
        {
            TreeView.OnGUI(rect);
            GUILayoutUtility.GetRect(rect.width, rect.height);
        }

        private static void PresentDebuggerDisabled()
        {
            GUILayout.FlexibleSpace();
            GUILayout.Label("To start debugging, enable the Reflex+ Debug Mode in the status bar.", Styles.LabelHorizontallyCentered);
            GUILayout.Label("Keep in mind that enabling Reflex+ Debug Mode will impact performance.", Styles.LabelHorizontallyCentered);
        }

        private void PresentDebuggerEnabled()
        {
            SearchBar(SearchBarRect);
            DoTreeView(MultiColumnTreeViewRect);

            GUILayout.Space(16);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Space(16);

                using (new GUILayout.VerticalScope())
                {
                    bindingStackTraceScrollPosition = GUILayout.BeginScrollView(bindingStackTraceScrollPosition);

                    PresentCallSite();

                    GUILayout.EndScrollView();
                    GUILayout.Space(16);
                }

                GUILayout.Space(16);
            }
        }

        private void PresentHierarchyFilters()
        {
            EditorGUI.BeginChangeCheck();
            ReflexPlusEditorSettings.ShowInternalBindings = GUILayout.Toggle(ReflexPlusEditorSettings.ShowInternalBindings, "Show Internal Bindings ");
            ReflexPlusEditorSettings.ShowInheritedBindings = GUILayout.Toggle(ReflexPlusEditorSettings.ShowInheritedBindings, "Show Inherited Bindings ");
            if (EditorGUI.EndChangeCheck())
            {
                Refresh();
            }
        }

        private void PresentStatusBar()
        {
            using (new EditorGUILayout.HorizontalScope(Styles.AppToolbar))
            {
                GUILayout.FlexibleSpace();
                PresentHierarchyFilters();

                var refreshIcon = EditorGUIUtility.IconContent("d_TreeEditor.Refresh");
                refreshIcon.tooltip = "Forces Tree View to Refresh";

                if (GUILayout.Button(refreshIcon, Styles.StatusBarIcon, GUILayout.Width(25)))
                {
                    Refresh();
                }

                var debuggerIcon = UnityScriptingDefineSymbols.IsDefined("REFLEX_PLUS_DEBUG")
                    ? EditorGUIUtility.IconContent("d_DebuggerEnabled")
                    : EditorGUIUtility.IconContent("d_DebuggerDisabled");

                debuggerIcon.tooltip = UnityScriptingDefineSymbols.IsDefined("REFLEX_PLUS_DEBUG")
                    ? "Reflex+ Debugger Enabled"
                    : "Reflex+ Debugger Disabled";

                if (GUILayout.Button(debuggerIcon, Styles.StatusBarIcon, GUILayout.Width(25)))
                {
                    UnityScriptingDefineSymbols.Toggle("REFLEX_PLUS_DEBUG", EditorUserBuildSettings.selectedBuildTargetGroup);
                }
            }
        }

        private void PresentCallSite()
        {
            var selection = TreeView.GetSelection();

            if (selection == null || selection.Count == 0)
            {
                return;
            }

            var item = TreeView.Find(selection.Single());

            if (item == null || item.Callsite == null)
            {
                return;
            }

            foreach (var callSite in item.Callsite)
            {
                PresentStackFrame(callSite.ClassName, callSite.FunctionName, callSite.Path, callSite.Line);
            }
        }

        private static void PresentStackFrame(string className,
            string functionName,
            string path,
            int line)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label($"{className}:{functionName}()  →", Styles.StackTrace);

                if (PresentLinkButton($"{path}:{line}"))
                {
                    UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(path, line);
                }
            }
        }

        private static bool PresentLinkButton(string label, params GUILayoutOption[] options)
        {
            var position = GUILayoutUtility.GetRect(new GUIContent(label), Styles.Hyperlink, options);
            position.y -= 3;
            Handles.color = Styles.Hyperlink.normal.textColor;
            Handles.DrawLine(new Vector3(position.xMin + EditorStyles.linkLabel.padding.left, position.yMax), 
                new Vector3(position.xMax - EditorStyles.linkLabel.padding.right, position.yMax));
            Handles.color = Color.white;
            EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
            return GUI.Button(position, label, Styles.Hyperlink);
        }
    }
}