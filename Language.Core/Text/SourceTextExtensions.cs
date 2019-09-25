using System;

namespace Pharmatechnik.Language.Text {

    public static class SourceTextExtensions {

        /// <summary>
        /// Liefert den Span vom Zeilenanfang bis zur angegebenen Position im Text.
        /// "Zeile mit Text"
        ///  ^-----^
        /// </summary>
        public static ReadOnlySpan<char> SliceFromLineStartToPosition(this SourceText sourceText, int toPosition) {
            var line = sourceText.GetTextLineAtPosition(toPosition);
            return sourceText.Slice(startIndex: line.Start, length: toPosition - line.Start);
        }

        /// <summary>
        /// Liefert den Span von der angegebenen Position im Text bis zum Zeilenende einschlieﬂlich des Zeilenvorschubs, falls vorhanden.
        /// "Zeile mit Text"
        ///        ^------^
        /// </summary>
        public static ReadOnlySpan<char> SliceFromPositionToLineEndIncludingLineBreak(this SourceText sourceText, int fromPosition) {
            var line = sourceText.GetTextLineAtPosition(fromPosition);
            return sourceText.Slice(startIndex: fromPosition, length: line.EndIncludingLineBreak - fromPosition);
        }

        /// <summary>
        /// Liefert den Span von der angegebenen Position im Text bis zum Zeilenende
        /// "Zeile mit Text"
        ///        ^------^
        /// </summary>
        public static ReadOnlySpan<char> SliceFromPositionToLineEnd(this SourceText sourceText, int fromPosition) {
            var line = sourceText.GetTextLineAtPosition(fromPosition);
            return sourceText.Slice(startIndex: fromPosition, length: line.End - fromPosition);
        }

    }

}