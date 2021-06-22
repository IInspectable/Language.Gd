#region Using Directives

using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static class TextSnapshotExtensions {

        public static SnapshotSpan GetFullSpan(this ITextSnapshot snapshot) {
            return new SnapshotSpan(snapshot, 0, snapshot.Length);
        }

        public static int LastIndexOf(this ITextSnapshot text, string value, int startIndex, bool caseSensitive) {

            var normalized = caseSensitive ? value : CaseInsensitiveComparison.ToLower(value);
            startIndex = startIndex + normalized.Length > text.Length
                ? text.Length - normalized.Length
                : startIndex;

            for (var i = startIndex; i >= 0; i--) {
                var match = true;
                for (var j = 0; j < normalized.Length; j++) {
                    // just use indexer of source text. perf of indexer depends on actual implementation of SourceText.
                    // * all of our implementation at editor layer should provide either O(1) or O(logn).
                    //
                    // only one implementation we have that could have bad indexer perf is CompositeText with heavily modified text
                    // at compiler layer but I believe that being used in find all reference will be very rare if not none.
                    if (!Match(normalized[j], text[i + j], caseSensitive)) {
                        match = false;
                        break;
                    }
                }

                if (match) {
                    return i;
                }
            }

            return -1;
        }

        public static int IndexOf(this ITextSnapshot text, string value, int startIndex, bool caseSensitive) {
            var length     = text.Length - value.Length;
            var normalized = caseSensitive ? value : CaseInsensitiveComparison.ToLower(value);

            for (var i = startIndex; i <= length; i++) {
                var match = true;
                for (var j = 0; j < normalized.Length; j++) {
                    // just use indexer of source text. perf of indexer depends on actual implementation of SourceText.
                    // * all of our implementation at editor layer should provide either O(1) or O(logn).
                    //
                    // only one implementation we have that could have bad indexer perf is CompositeText with heavily modified text
                    // at compiler layer but I believe that being used in find all reference will be very rare if not none.
                    if (!Match(normalized[j], text[i + j], caseSensitive)) {
                        match = false;
                        break;
                    }
                }

                if (match) {
                    return i;
                }
            }

            return -1;
        }

        static bool Match(char normalizedLeft, char right, bool caseSensitive) {
            return caseSensitive ? normalizedLeft == right : normalizedLeft == CaseInsensitiveComparison.ToLower(right);
        }

    }

}