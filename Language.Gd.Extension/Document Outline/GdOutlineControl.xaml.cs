using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.DocumentOutline;
using Pharmatechnik.Language.Gd.Extension.Common;
using Pharmatechnik.Language.Gd.Extension.Imaging;

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
                TreeView.Visibility  = System.Windows.Visibility.Collapsed;
                Watermark.Visibility = System.Windows.Visibility.Visible;
            } else {
                TreeView.Visibility  = System.Windows.Visibility.Visible;
                Watermark.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private bool IsNavigatingToOutline { get; set; }

        IDisposable NavigatingToOutline() {
            return new ScopedValue<bool>(getter: () => IsNavigatingToOutline, setter: v => IsNavigatingToOutline = v, value: true);
        }

        internal void NavigateToOutline([CanBeNull] OutlineElement outlineElement) {
            using (NavigatingToOutline()) {

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
                var outlineElement = (OutlineElement) ((TreeViewItem) o).Tag;
                if (!IsNavigatingToOutline) {
                    RequestNavigateToSource?.Invoke(this, new RequestNavigateToEventArgs(outlineElement));
                }

                e.Handled = true;
            };

            // TODO Bai NavigateToOutline sollte das aber dennoch funktionieren
            item.RequestBringIntoView += (o, e) => { e.Handled = true; };

            foreach (var child in outline.Children) {
                AddOutlineElement(item, child);
            }

            itemCollection.Add(item);
            _flattenTree[outline] = item;

        }

    }

}