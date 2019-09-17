using System;

namespace Pharmatechnik.Language.Text {

    public static class SourceTextLineExtensions {

        public static string GetIndentAsSpaces(this SourceTextLine sourceText, int tabSize) {

            var startColumn = sourceText.GetIndentationColumn(tabSize);

            return new String(' ', startColumn);
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
        public static int GetColumnForOffset(this SourceTextLine sourceText, int tabSize, int charPositionInLine) {
            return sourceText.Span.GetColumnForOffset(tabSize, charPositionInLine);
        }

        /// <summary>
        /// Liefert den Spaltenindex (beginnend bei 0) für das erste Signifikante Zeichen in der angegebenen Zeile.
        /// Als nicht signifikant gelten alle Arten von Leerzeichen. Dabei werden Tabulatoren entsprechend umgerechnet.
        /// </summary>
        /// <example>
        /// Gegeben sei folgende Zeile mit gemischten Leerzeichen (o) und Tabulatoren (->) mit einer Tabulatorweite 
        /// von 4 und anschließendem Text (T):
        /// --->oo->TTTTTT
        /// --------^ 
        /// Der Signifikante Spaltenindex für diese Zeile ist 8.
        /// </example>
        public static int GetIndentationColumn(this SourceTextLine sourceText, int tabSize) {
            return sourceText.Span.GetIndentationColumn(tabSize);
        }

    }

}