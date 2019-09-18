using System;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.DocumentOutline;
using Pharmatechnik.Language.Gd.Extension.Imaging;

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    using System.Windows.Controls;

    partial class GdOutlineControl: UserControl {

        public GdOutlineControl() {
            InitializeComponent();

        }

        public event EventHandler<RequestNavigateToEventArgs> RequestNavigateToSource;

        internal void ShowOutline([CanBeNull] OutlineData outlineData) {

            AddOutlineElement(null, outlineData?.OutlineElement);
        }

        private void AddOutlineElement(TreeViewItem parent, [CanBeNull] OutlineElement outline) {

            if (parent == null) {
                TreeView.Items.Clear();
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
                RequestNavigateToSource?.Invoke(this, new RequestNavigateToEventArgs(outlineElement));
                e.Handled = true;
            };

            foreach (var child in outline.Children) {
                AddOutlineElement(item, child);
            }

            itemCollection.Add(item);

        }

    }

}