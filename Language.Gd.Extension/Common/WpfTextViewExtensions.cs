#region Using Directives

using System.Linq;
using System.Windows;
using System.Windows.Input;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static class WpfTextViewExtensions {

        public static SnapshotPoint? GetBufferPositionAtMousePosition(this IWpfTextView textView) {

            Point position = Mouse.GetPosition(textView.VisualElement);

            var viewportPoint = textView.ToViewportPoint(position);
            var line          = textView.TextViewLines.GetTextViewLineContainingYCoordinate(viewportPoint.Y);
            var bufferPos     = line?.GetBufferPositionFromXCoordinate(viewportPoint.X, true);

            return bufferPos;
        }

        public static Point ToViewportPoint(this IWpfTextView textView, Point position) {
            return new Point(position.X + textView.ViewportLeft, position.Y + textView.ViewportTop);
        }

        [CanBeNull]
        public static ITagSpan<T> MapToSingleSnapshotSpan<T>(this IWpfTextView textView, IMappingTagSpan<T> mappingTagSpan) where T : ITag {

            if (mappingTagSpan == null || textView.TextSnapshot == null) {
                return null;
            }

            var tagSpans = mappingTagSpan.Span.GetSpans(textView.TextSnapshot);
            if (!tagSpans.Any()) {
                return null;
            }

            return new TagSpan<T>(tagSpans[0], mappingTagSpan.Tag);
        }

        public static void PrepareSizeToFit(this IWpfTextView view) {
            view.LayoutChanged += (s, e) => {
                ThreadHelper.JoinableTaskFactory.RunAsync(async () => {

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

                    view.VisualElement.Height = view.LineHeight * view.TextBuffer.CurrentSnapshot.LineCount;
                    double width = view.VisualElement.Width;
                    if (!IsNormal(view.MaxTextRightCoordinate))
                        return;
                    if (IsNormal(width) && view.MaxTextRightCoordinate <= width)
                        return;

                    view.VisualElement.Width = view.MaxTextRightCoordinate;
                });
            };
        }

        static bool IsNormal(double value) {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }

    }

}