#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static partial class TextViewExtensions {

        public static ISet<IContentType> GetContentTypes(this ITextView textView) {
            return new HashSet<IContentType>(
                textView.BufferGraph.GetTextBuffers(_ => true).Select(b => b.ContentType));
        }

        public static ITextBuffer GetBufferContainingCaret(this ITextView textView, string contentType = GdLanguageContentDefinitions.ContentType) {
            var point = GetCaretPoint(textView, s => s.ContentType.IsOfType(contentType));
            return point?.Snapshot.TextBuffer;
        }

        public static SnapshotPoint? GetCaretPoint(this ITextView textView, Predicate<ITextSnapshot> match) {
            var caret = textView.Caret.Position;
            var span  = textView.BufferGraph.MapUpOrDownToFirstMatch(new SnapshotSpan(caret.BufferPosition, 0), match);
            return span?.Start;
        }

        public static SnapshotPoint? GetCaretPoint(this ITextView textView) {
            var caretPoint = textView.Caret.Position.Point.GetPoint(textView.TextBuffer, textView.Caret.Position.Affinity);
            return caretPoint;
        }

        /// <summary>
        /// Gets or creates a view property that would go away when view gets closed
        /// </summary>
        public static TProperty GetOrCreateAutoClosingProperty<TProperty, TTextView>(
            this TTextView textView,
            Func<TTextView, TProperty> valueCreator) where TTextView : ITextView {
            return textView.GetOrCreateAutoClosingProperty(typeof(TProperty), valueCreator);
        }

        /// <summary>
        /// Gets or creates a view property that would go away when view gets closed
        /// </summary>
        public static TProperty GetOrCreateAutoClosingProperty<TProperty, TTextView>(
            this TTextView textView,
            object key,
            Func<TTextView, TProperty> valueCreator) where TTextView : ITextView {
            GetOrCreateAutoClosingProperty(textView, key, valueCreator, out var value);
            return value;
        }

        /// <summary>
        /// Gets or creates a view property that would go away when view gets closed
        /// </summary>
        public static bool GetOrCreateAutoClosingProperty<TProperty, TTextView>(
            this TTextView textView,
            object key,
            Func<TTextView, TProperty> valueCreator,
            out TProperty value) where TTextView : ITextView {
            return AutoClosingViewProperty<TProperty, TTextView>.GetOrCreateValue(textView, key, valueCreator, out value);
        }

        public static void SetSelection(this ITextView textView, SnapshotSpan span, bool isReversed = false) {
            var spanInView = textView.GetSpanInView(span).Single();
            textView.Selection.Select(spanInView, isReversed);
            textView.Caret.MoveTo(isReversed ? spanInView.Start : spanInView.End);
        }

        public static NormalizedSnapshotSpanCollection GetSpanInView(this ITextView textView, SnapshotSpan span) {
            return textView.BufferGraph.MapUpToSnapshot(span, SpanTrackingMode.EdgeInclusive, textView.TextSnapshot);
        }

        public static bool TryMoveCaretToAndEnsureVisible(this ITextView textView, SnapshotPoint point, IOutliningManagerService outliningManagerService = null, EnsureSpanVisibleOptions ensureSpanVisibleOptions = EnsureSpanVisibleOptions.None) {
            return textView.TryMoveCaretToAndEnsureVisible(new VirtualSnapshotPoint(point), outliningManagerService, ensureSpanVisibleOptions);
        }

        public static bool TryMoveCaretToAndEnsureVisible(this ITextView textView, VirtualSnapshotPoint point, IOutliningManagerService outliningManagerService = null, EnsureSpanVisibleOptions ensureSpanVisibleOptions = EnsureSpanVisibleOptions.None) {
            if (textView.IsClosed) {
                return false;
            }

            var pointInView = textView.GetPositionInView(point.Position);

            if (!pointInView.HasValue) {
                return false;
            }

            // If we were given an outlining service, we need to expand any outlines first, or else
            // the Caret.MoveTo won't land in the correct location if our target is inside a
            // collapsed outline.
            var outliningManager = outliningManagerService?.GetOutliningManager(textView);

            outliningManager?.ExpandAll(new SnapshotSpan(pointInView.Value, length: 0), match: _ => true);

            var newPosition = textView.Caret.MoveTo(new VirtualSnapshotPoint(pointInView.Value, point.VirtualSpaces));

            // We use the caret's position in the view's current snapshot here in case something 
            // changed text in response to a caret move (e.g. line commit)
            var spanInView = new SnapshotSpan(newPosition.BufferPosition, 0);
            textView.ViewScroller.EnsureSpanVisible(spanInView, ensureSpanVisibleOptions);

            return true;
        }

        public static SnapshotPoint? GetPositionInView(this ITextView textView, SnapshotPoint point) {
            return textView.BufferGraph.MapUpToSnapshot(point, PointTrackingMode.Positive, PositionAffinity.Successor, textView.TextSnapshot);
        }

        public static TextEditorSettings GetEditorSettings(this ITextView textView) {
            return new TextEditorSettings(
                tabSize: textView.Options.GetTabSize(),
                newLine: textView.Options.GetNewLineCharacter());
        }

    }

}