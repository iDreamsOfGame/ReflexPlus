using System;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Assertions;

namespace ReflexPlusEditor.DebuggingWindow
{
    internal class MultiColumnTreeView : TreeViewWithTreeModel<MyTreeElement>
    {
        private const float RowHeight = 20f;

        private const float ToggleWidth = 18f;

        private enum Column
        {
            Hierarchy,

            Kind,

            Lifetime,

            Calls,
        }

        public MultiColumnTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader, TreeModel<MyTreeElement> model)
            : base(state, multiColumnHeader, model)
        {
            rowHeight = RowHeight;
            columnIndexForTreeFoldouts = 0;
            showAlternatingRowBackgrounds = true;
            showBorder = true;
            customFoldoutYOffset = (RowHeight - EditorGUIUtility.singleLineHeight) * 0.5f; // center foldout in the row since we also center content. See RowGUI
            extraSpaceBeforeIconAndLabel = ToggleWidth;
            Reload();
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            var item = (TreeViewItem<MyTreeElement>)args.item;

            for (var i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), item, (Column)args.GetColumn(i));
            }
        }

        private Texture2D texture;

        private void TryInitTexture()
        {
            if (!texture)
            {
                texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0.5f));
                texture.Apply();
            }
        }

        private void CellGUI(Rect cellRect,
            TreeViewItem<MyTreeElement> item,
            Column column)
        {
            TryInitTexture();
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (column)
            {
                case Column.Hierarchy:
                    DrawItemIcon(cellRect, item);
                    cellRect.xMin += 20; // Icon size (16) + 4 of padding

                    if (item.Data.Contracts is { Length: > 0 })
                    {
                        DrawContracts(item, cellRect, item.Data.Contracts);
                    }
                    else
                    {
                        DrawName(item, cellRect, item.Data.Name);
                    }
                    break;
                
                case Column.Calls:
                    GUI.Label(cellRect, item.Data.Resolutions.Invoke());
                    break;
                
                case Column.Kind:
                    GUI.Label(cellRect, item.Data.Kind);
                    break;
                
                case Column.Lifetime:
                    GUI.Label(cellRect, item.Data.ResolutionType);
                    break;
            }
        }

        private void DrawName(TreeViewItem item, Rect area, string name)
        {
            area.xMin += GetContentIndent(item);
            GUI.Label(area, name, Styles.RichTextLabel);
        }

        private void DrawContracts(TreeViewItem<MyTreeElement> item, Rect rect, string[] contracts)
        {
            if (contracts == null || contracts.Length == 0 || string.IsNullOrEmpty(contracts[0]))
                return;

            rect.y += 1;

            var style = new GUIStyle("CN CountBadge") // CN CountBadge, AssetLabel, AssetLabel Partial
            {
                wordWrap = false,
                stretchWidth = false,
                stretchHeight = false,
                fontStyle = FontStyle.Bold,
                fontSize = 11
            };

            rect.xMin += GetContentIndent(item);

            // Clipping group
            GUI.BeginGroup(rect);
            {
                var labelXOffset = 0.0f;
                foreach (var contract in contracts)
                {
                    var content = new GUIContent($"{contract}");
                    var labelWidth = style.CalcSize(content).x;

                    // Draw the label within the bounds of the rect
                    var labelRect = new Rect(labelXOffset, 0, labelWidth, rect.height);
                    GUI.Label(labelRect, content, style);

                    // Move the rect for the next contract
                    labelXOffset += labelWidth + 4;

                    // Stop drawing if the labels go beyond the column's width
                    if (labelXOffset > rect.width)
                        break;
                }
            }
            GUI.EndGroup();
        }

        private void DrawItemIcon(Rect area, TreeViewItem<MyTreeElement> item)
        {
            area.xMin += GetContentIndent(item);

            // Clipping group
            GUI.BeginGroup(area);
            {
                // Draw the icon within the bounds of the area
                var iconRect = new Rect(0, 0, 16, area.height);
                GUI.DrawTexture(iconRect, item.Data.Icon, ScaleMode.ScaleToFit);
            }
            GUI.EndGroup();
        }

        private void DrawItemNameColumn(Rect area, TreeViewItem<MyTreeElement> item, ref RowGUIArgs args)
        {
            DrawItemIcon(area, item);
            area.x += 4;
            args.rowRect = area;
            base.RowGUI(args);
        }

        protected override bool CanMultiSelect(TreeViewItem item) => false;

        public static MultiColumnHeaderState CreateDefaultMultiColumnHeaderState()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    canSort = false,
                    headerContent = new GUIContent(nameof(Column.Hierarchy)),
                    headerTextAlignment = TextAlignment.Left,
                    width = 280,
                    minWidth = 60,
                    autoResize = true,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    canSort = false,
                    headerContent = new GUIContent(nameof(Column.Kind)),
                    headerTextAlignment = TextAlignment.Left,
                    width = 64,
                    minWidth = 64,
                    maxWidth = 64,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    canSort = false,
                    headerContent = new GUIContent(nameof(Column.Lifetime)),
                    headerTextAlignment = TextAlignment.Left,
                    width = 64,
                    minWidth = 64,
                    maxWidth = 64,
                    autoResize = false,
                    allowToggleVisibility = false
                },
                new MultiColumnHeaderState.Column
                {
                    canSort = false,
                    headerContent = new GUIContent(nameof(Column.Calls)),
                    headerTextAlignment = TextAlignment.Left,
                    width = 38,
                    minWidth = 38,
                    autoResize = false
                },
            };

            Assert.AreEqual(columns.Length, Enum.GetValues(typeof(Column)).Length, "Number of columns should match number of enum values");
            return new MultiColumnHeaderState(columns);
        }
    }
}