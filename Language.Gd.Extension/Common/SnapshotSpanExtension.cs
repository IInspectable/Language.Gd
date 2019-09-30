#region Using Directives

using Microsoft.VisualStudio.Text;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static class SnapshotSpanExtension {

        public static TextExtent ToTextExtent(this SnapshotSpan span) {
            return new TextExtent(span.Start, span.Length);
        }

    }

}