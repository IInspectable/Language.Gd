#region Using Directives

using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static class TextSelectionExtensions {

        public static NormalizedSnapshotSpanCollection GetSnapshotSpansOnBuffer(this ITextSelection selection, ITextBuffer subjectBuffer) {
            var list = new List<SnapshotSpan>();
            foreach (var snapshotSpan in selection.SelectedSpans) {
                list.AddRange(selection.TextView.BufferGraph.MapDownToBuffer(snapshotSpan, SpanTrackingMode.EdgeExclusive, subjectBuffer));
            }

            return new NormalizedSnapshotSpanCollection(list);
        }

    }

}