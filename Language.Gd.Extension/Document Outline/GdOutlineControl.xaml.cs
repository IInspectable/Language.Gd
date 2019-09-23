#region Using Directives

using System;
using System.Collections.Generic;
using System.Windows;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.DocumentOutline;
using Pharmatechnik.Language.Gd.Extension.Common;
using Pharmatechnik.Language.Gd.Extension.Imaging;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    using System.Windows.Controls;

    partial class GdOutlineControl: UserControl {

        readonly Dictionary<OutlineElement, TreeViewItem> _flattenTree;

        public GdOutlineControl() {
            InitializeComponent();
            _flattenTree = new Dictionary<OutlineElement, TreeViewItem>();

        }

        public event EventHandler<RequestNavigateToEventArgs> RequestNavigateToSource;

        internal void ShowOutline([CanBeNull] OutlineData outlineData) {

            AddOutlineElement(null, outlineData?.OutlineElement);

            if (TreeView.Items.Count == 0) {
                TreeView.Visibility  = Visibility.Collapsed;
                Watermark.Visibility = Visibility.Visible;
            } else {
                TreeView.Visibility  = Visibility.Visible;
                Watermark.Visibility = Visibility.Collapsed;
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

        private void AddOutlineElement(TreeViewItem parent, [CanBeNull] OutlineElement outline) {

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
                    TextContent = {Content = outline.DisplayName}
                },
                Tag        = outline,
                IsExpanded = true,
            };

            item.Selected += (o, e) => {

                e.Handled = true;

                if (_isNavigatingToOutline) {
                    return;
                }

                var outlineElement = (OutlineElement) ((TreeViewItem) o).Tag;
                RequestNavigateToSource?.Invoke(this, new RequestNavigateToEventArgs(outlineElement));

            };

            item.RequestBringIntoView += OnItemRequestBringIntoView;

            foreach (var child in outline.Children) {
                AddOutlineElement(item, child);
            }

            itemCollection.Add(item);
            _flattenTree[outline] = item;

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

    }

}