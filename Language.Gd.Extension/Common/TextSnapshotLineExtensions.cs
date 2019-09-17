#region Using Directives

using System;

using Microsoft.VisualStudio.Text;

using Pharmatechnik.Language.CodeAnalysis;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    static class TextSnapshotLineExtensions {

        /// <summary>
        /// Returns the first non-whitespace position on the given line, or null if 
        /// the line is empty or contains only whitespace.
        /// </summary>
        public static int? GetFirstNonWhitespacePosition(this ITextSnapshotLine line) {

            var text = line.GetText();

            for (int i = 0; i < text.Length; i++) {
                if (!char.IsWhiteSpace(text[i])) {
                    return line.Start + i;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the last non-whitespace position on the given line, or null if 
        /// the line is empty or contains only whitespace.
        /// </summary>
        public static int? GetLastNonWhitespacePosition(this ITextSnapshotLine line) {

            var text = line.GetText();

            for (int i = text.Length - 1; i >= 0; i--) {
                if (!char.IsWhiteSpace(text[i])) {
                    return line.Start + i;
                }
            }

            return null;
        }

        /// <summary>
        /// Liefert den Spaltenindex (beginnend bei 0) für den angegebenen Offset vom Start der Zeile. 
        /// Es werden Tabulatoren entsprechend eingerechnet.
        /// </summary>
        /// <example>
        /// Gegeben sei folgende Zeile mit gemischten Leerzeichen (o) und Tabulatoren (->) mit einer Tabulatorweite 
        /// von 4 und anschließendem Text (T). Der angeforderte Offset ist 4:
        /// TT->--->TTTTTT
        /// ^^-^---^
        /// Der Spaltenindex für den Zeichenindex 4 ist 8 (man beachte die 2 Tabulatoren!).
        /// </example>
        public static int GetColumnForOffset(this ITextSnapshotLine line, int tabSize, int offset) {
            var text = line.GetText();
            return text.GetColumnFromPosition(tabSize, offset);
        }

        /// <summary>
        /// Liefert den Spaltenindex (beginnend bei 0) für das erste signifikante Zeichen in der angegebenen Zeile.
        /// Als nicht signifikant gelten alle Arten von Leerzeichen. Dabei werden Tabulatoren entsprechend umgerechnet.
        /// Null, wenn es kein signifikantes Zeichen in der Zeile gibt.
        /// </summary>
        /// <example>
        /// Gegeben sei folgende Zeile mit gemischten Leerzeichen (o) und Tabulatoren (->) mit einer Tabulatorweite 
        /// von 4 und anschließendem Text (T):
        /// --->oo->TTTTTT
        /// --------^ 
        /// Der Signifikante Spaltenindex für diese Zeile ist 8.
        /// </example>
        public static int? GetIndentationColumn(this ITextSnapshotLine line, int tabSize) {
            return line.GetText().GetIndentationColumn(tabSize);
        }

        /// <summary>
        /// Liefert den Offset beginnend bei 0 (= Anfang der Zeile), der zum angegbenen nullbasierten
        /// Spaltenindex führt.
        /// </summary>
        /// <example>
        /// Gegeben sei folgende Zeile mit gemischten Leerzeichen (o) und Tabulatoren (->) mit einer Tabulatorweite 
        /// von 4 und anschließendem Text (T). Der angeforderte Spaltenindex ist 8:
        /// TT->--->TTTTTT
        /// ^^-^---^
        /// Der Offset für den angeforderten Spaltenindex 8 ist 4 (man beachte die 2 Tabulatoren!).
        /// </example>
        public static int GetOffsetForColumn(this ITextSnapshotLine line, int column, int tabSize) {
            var text          = line.GetText();
            int offset        = 0;
            int currentColumn = 0;
            for (int index = 0; index < text.Length; index++) {
                var c = text[index];
                if (currentColumn >= column) {
                    break;
                }

                if (c == '\t') {
                    currentColumn += tabSize - currentColumn % tabSize;
                } else {
                    currentColumn++;
                }

                offset++;
            }

            return offset;
        }

        /// <summary>
        /// Determines whether the specified line is empty or contains whitespace only.
        /// </summary>
        public static bool IsEmptyOrWhitespace(this ITextSnapshotLine line) {

            var text = line.GetText();

            for (int i = 0; i < text.Length; i++) {
                if (!char.IsWhiteSpace(text[i])) {
                    return false;
                }
            }

            return true;
        }

        public static ITextSnapshotLine GetPreviousMatchingLine(this ITextSnapshotLine line, Func<ITextSnapshotLine, bool> predicate) {

            if (line.LineNumber <= 0) {
                return null;
            }

            var snapshot = line.Snapshot;
            for (int lineNumber = line.LineNumber - 1; lineNumber >= 0; lineNumber--) {
                var currentLine = snapshot.GetLineFromLineNumber(lineNumber);
                if (!predicate(currentLine)) {
                    continue;
                }

                return currentLine;
            }

            return null;
        }

    }

}