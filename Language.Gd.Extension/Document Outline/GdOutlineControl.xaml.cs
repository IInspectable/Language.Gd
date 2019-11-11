#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Language.Gd.DocumentOutline;
using Pharmatechnik.Language.Gd.Extension.Classification;
using Pharmatechnik.Language.Gd.Extension.Common;
using Pharmatechnik.Language.Gd.Extension.Imaging;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    partial class GdOutlineControl: UserControl {

        private readonly TextBlockBuilderService _textBlockBuilderService;

        readonly Dictionary<OutlineElement, TreeViewItem> _flattenTree;

        internal GdOutlineControl(TextBlockBuilderService textBlockBuilderService) {
            _textBlockBuilderService = textBlockBuilderService;
            InitializeComponent();
            _flattenTree = new Dictionary<OutlineElement, TreeViewItem>();

        }

        public event EventHandler<RequestNavigateToEventArgs> RequestNavigateToSource;
        public event EventHandler<RequestNavigateToEventArgs> RequestNavigateToSecondaryLocation;

        public bool NavigateDown() {
            return Navigate(up: false);
        }

        public bool NavigateUp() {
            return Navigate(up: true);
        }

        bool Navigate(bool up) {

            var items = _flattenTree.Values.ToList();

            if (items.Count == 0) {
                return false;
            }

            var currentIndex = items.IndexOf(TreeView.SelectedItem as TreeViewItem);

            if (up) {
                // keine Selektion
                if (currentIndex < 0) {
                    currentIndex = items.Count;
                }

                currentIndex -= 1;

                if (currentIndex < 0) {
                    currentIndex = items.Count - 1;
                }
            } else {
                // keine Selektion
                if (currentIndex < 0) {
                    currentIndex = -1;
                }

                currentIndex += 1;

                if (currentIndex >= items.Count) {
                    currentIndex = 0;
                }
            }

            items[currentIndex].IsSelected = true;
            return true;

        }

        internal void ShowOutline([CanBeNull] OutlineData outlineData, [CanBeNull] Regex searchPattern) {

            ThreadHelper.ThrowIfNotOnUIThread();

            AddOutlineElement(null, outlineData?.OutlineElement, searchPattern);

            if (TreeView.Items.Count == 0) {
                TreeView.Visibility  = Visibility.Collapsed;
                Watermark.Visibility = Visibility.Visible;
            } else {
                TreeView.Visibility  = Visibility.Visible;
                Watermark.Visibility = Visibility.Collapsed;
            }
        }

        public bool HasItems => TreeView.Items.Count > 0;

        public void CollapseAll() {
            ExpandCollapseItems(expand: false);
        }

        public void ExpandAll() {
            ExpandCollapseItems(expand: true);
        }

        void ExpandCollapseItems(bool expand) {
            foreach (var item in TreeView.Items.OfType<TreeViewItem>()) {
                ExpandCollapseItems(item);
            }

            void ExpandCollapseItems(TreeViewItem item) {
                item.IsExpanded = expand;

                foreach (TreeViewItem childItem in item.Items) {
                    childItem.IsExpanded = expand;

                    if (childItem.HasItems)
                        ExpandCollapseItems(childItem);
                }
            }
        }

        readonly ScopedProperty<bool> _isNavigatingToOutline = ScopedProperty.Boolean();

        internal void NavigateToOutline([CanBeNull] OutlineElement outlineElement) {
            using (_isNavigatingToOutline.Enter()) {

                if (outlineElement != null && _flattenTree.TryGetValue(outlineElement, out var item)) {
                    item.IsSelected = true;
                    item.BringIntoView();
                    return;
                }

                if (TreeView.SelectedItem is TreeViewItem prevSel) {
                    prevSel.IsSelected = false;
                }
            }
        }

        private void AddOutlineElement(TreeViewItem parent, [CanBeNull] OutlineElement outline, [CanBeNull] Regex searchPattern) {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (parent == null) {
                TreeView.Items.Clear();
                _flattenTree.Clear();
            }

            if (outline == null) {
                return;
            }

            var itemCollection = parent?.Items ?? TreeView.Items;

            var item = new TreeViewItem {
                Header = new OutlineItemControl {
                    CrispImage = {
                        Moniker = GdImageMonikers.GetMoniker(outline.Glyph)
                    },
                    TextContent = {Content = MakeItemContent(outline, searchPattern, out var hasMatch)}
                },
                Tag        = outline,
                IsExpanded = true,
            };

            _flattenTree[outline] = item;

            foreach (var child in outline.Children) {
                AddOutlineElement(item, child, searchPattern);
            }

            var isMatch = searchPattern == null || hasMatch;

            if (isMatch || item.Items.Count > 0) {

                item.Selected             += OnItemSelected;
                item.RequestBringIntoView += OnItemRequestBringIntoView;
                item.KeyDown              += OnTreeViewItemKeyDown;
                item.MouseDoubleClick     += OnItemDoubleClick;

                itemCollection.Add(item);

            } else {
                _flattenTree.Remove(outline);
            }

        }

        private TextBlock MakeItemContent(OutlineElement outline, [CanBeNull] Regex searchPattern, out bool hasMatch) {
            ThreadHelper.ThrowIfNotOnUIThread();
            return _textBlockBuilderService.ToTextBlock(outline.DisplayParts, searchPattern, out hasMatch);
        }

        readonly ScopedProperty<bool> _isHandlingOnItemRequestBringIntoView = ScopedProperty.Boolean();

        void OnItemRequestBringIntoView(object sender, RequestBringIntoViewEventArgs e) {

            if (_isHandlingOnItemRequestBringIntoView) {
                return;
            }

            using (_isHandlingOnItemRequestBringIntoView.Enter()) {

                if (sender is TreeViewItem tvi &&
                    tvi.Header is OutlineItemControl oic) {

                    tvi.UpdateLayout();
                    // See: https://stackoverflow.com/questions/3225940/prevent-automatic-horizontal-scroll-in-treeview/34269542#42238409
                    var newTargetRect = new Rect(x: -1000, y: 0, width: oic.ActualWidth + 1000, height: oic.ActualHeight);
                    tvi.BringIntoView(newTargetRect);

                    e.Handled = true;

                }
            }
        }

        private void OnTreeViewItemKeyDown(object sender, KeyEventArgs e) {

            if (!(e.Source is TreeViewItem item) ||
                Keyboard.Modifiers != ModifierKeys.None) {
                return;
            }

            switch (e.Key) {

                case Key.Space:
                    item.IsExpanded = !item.IsExpanded;
                    e.Handled       = true;
                    break;

            }
        }

        private void OnItemDoubleClick(object sender, MouseButtonEventArgs e) {
            var tvi = sender as TreeViewItem;

            if (tvi?.IsSelected == false || _isNavigatingToOutline) {
                return;
            }

            e.Handled = true;
            var outlineElement = (OutlineElement) ((TreeViewItem) sender).Tag;
            RequestNavigateToSecondaryLocation?.Invoke(this, new RequestNavigateToEventArgs(outlineElement));
        }

        private void OnItemSelected(object sender, RoutedEventArgs e) {
            e.Handled = true;

            if (_isNavigatingToOutline) {
                return;
            }

            var outlineElement = (OutlineElement) ((TreeViewItem) sender).Tag;
            RequestNavigateToSource?.Invoke(this, new RequestNavigateToEventArgs(outlineElement));
        }

    }

}