using System;
using System.Collections.Generic;
using ReflexPlus.Logging;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace ReflexPlusEditor.DebuggingWindow
{
    internal class TreeViewWithTreeModel<T> : TreeView where T : TreeElement
    {
        private TreeModel<T> treeModel;

        private readonly List<TreeViewItem> rows = new(100);

        public TreeViewWithTreeModel(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<T> model)
            : base(state, multiColumnHeader)
        {
            Init(model);
        }

        private void Init(TreeModel<T> model)
        {
            treeModel = model;
        }

        public T Find(int id)
        {
            return treeModel.Find(id);
        }

        protected override TreeViewItem BuildRoot()
        {
            var depthForHiddenRoot = -1;
            return new TreeViewItem<T>(treeModel.Root.Id, depthForHiddenRoot, treeModel.Root.Name, treeModel.Root);
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            if (treeModel.Root == null)
            {
                ReflexPlusLogger.Log("tree model root is null. did you call SetData()?", LogLevel.Error);
            }

            rows.Clear();
            if (!string.IsNullOrEmpty(searchString))
            {
                Search(treeModel.Root, searchString, rows);
            }
            else
            {
                if (treeModel.Root is { HasChildren: true })
                {
                    AddChildrenRecursive(treeModel.Root, 0, rows);
                }
            }

            // We still need to setup the child parent information for the rows since this 
            // information is used by the TreeView internal logic (navigation, dragging etc)
            SetupParentsAndChildrenFromDepths(root, rows);

            return rows;
        }

        private void AddChildrenRecursive(T parent, int depth, IList<TreeViewItem> newRows)
        {
            foreach (var treeElement in parent.Children)
            {
                var child = (T)treeElement;
                var item = new TreeViewItem<T>(child.Id, depth, child.Name, child);
                newRows.Add(item);

                if (child.HasChildren)
                {
                    if (IsExpanded(child.Id))
                    {
                        AddChildrenRecursive(child, depth + 1, newRows);
                    }
                    else
                    {
                        item.children = CreateChildListForCollapsedParent();
                    }
                }
            }
        }

        private static void Search(T searchFromThis, string search, List<TreeViewItem> result)
        {
            if (string.IsNullOrEmpty(search))
            {
                throw new ArgumentException("Invalid search: cannot be null or empty", nameof(search));
            }

            if (searchFromThis == null || searchFromThis.Children == null)
            {
                return;
            }

            const int itemDepth = 0; // tree is flattened when searching

            var stack = new Stack<T>();
            foreach (var element in searchFromThis.Children)
            {
                stack.Push((T)element);
            }

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                // Matches search?
                if (current.Name.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    result.Add(new TreeViewItem<T>(current.Id, itemDepth, current.Name, current));
                }

                if (current.Children is { Count: > 0 })
                {
                    foreach (var element in current.Children)
                    {
                        stack.Push((T)element);
                    }
                }
            }

            SortSearchResult(result);
        }

        private static void SortSearchResult(List<TreeViewItem> rows)
        {
            rows.Sort((x, y) => EditorUtility.NaturalCompare(x.displayName, y.displayName));
        }

        protected override IList<int> GetAncestors(int id)
        {
            return treeModel.GetAncestors(id);
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            return treeModel.GetDescendantsThatHaveChildren(id);
        }
    }
}