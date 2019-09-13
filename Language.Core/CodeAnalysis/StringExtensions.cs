using System;
using System.Linq;
using System.Text;

namespace Pharmatechnik.Language.CodeAnalysis {

    public static class StringExtensions {

        /// <summary>
        /// Liefert den Spaltenindex (beginnend bei 0) für die angegebenen Position der Zeile. 
        /// Es werden Tabulatoren entsprechend eingerechnet.
        /// </summary>
        /// <example>
        /// Gegeben sei folgende Zeile mit gemischten Leerzeichen (o) und Tabulatoren (->) mit einer Tabulatorweite 
        /// von 4 und anschließendem Text (T). Die angeforderte Position ist 4:
        /// TT->--->TTTTTT
        /// ^^-^---^
        /// Der Spaltenindex für den Zeichenindex 4 ist 8 (man beachte die 2 Tabulatoren!).
        /// </example>
        public static int GetColumnFromPosition(this string text, int tabSize, int position) {
            var column = 0;
            for (int index = 0; index < position; index++) {
                var c = text[index];
                if (c == '\t') {
                    column += tabSize - column % tabSize;
                } else {
                    column++;
                }
            }

            return column;
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
        public static int? GetFirstNonWhitespaceColumn(this string text, int tabSize) {

            bool hasSignificantContent = false;
            int  column                = 0;

            for (int index = 0; index < text.Length; index++) {
                var c = text[index];

                if (c == '\t') {
                    column += tabSize - column % tabSize;
                } else if (Char.IsWhiteSpace(c)) {
                    column++;
                } else {
                    hasSignificantContent = true;
                    break;
                }
            }

            return hasSignificantContent ? column : (int?) null;
        }

        public static int GetFirstNonWhitespaceIndex(this string line) {

            for (var i = 0; i < line.Length; i++) {
                if (!char.IsWhiteSpace(line[i])) {
                    return i;
                }
            }

            return -1;
        }

        public static int GetFirstNonSpaceOrTabIndex(this string text) {

            for (int i = 0; i < text.Length; i++) {
                if (text[i] != ' ' && text[i] != '\t') {
                    return i;
                }
            }

            return -1;
        }

        public static string GetLeadingWhitespace(this string lineText) {

            var length = lineText.GetFirstNonWhitespaceIndex();

            return length >= 0
                ? lineText.Substring(0, length)
                : lineText;
        }

        public static string GetFirstLineText(this string text) {
            var lineBreak = text.IndexOf('\n');
            return lineBreak < 0 ? text : text.Substring(0, lineBreak + 1);

        }

        public static string GetLastLineText(this string text) {
            var lineBreak = text.LastIndexOf('\n');
            return lineBreak < 0 ? text : text.Substring(lineBreak + 1);

        }

        public static bool ContainsLineBreak(this string text) {
            foreach (var ch in text) {
                if (ch == '\n' || ch == '\r') {
                    return true;
                }
            }

            return false;
        }

        public static int GetNumberOfLineBreaks(this string text) {
            var lineBreaks = 0;
            for (var i = 0; i < text.Length; i++) {
                if (text[i] == '\n') {

                    lineBreaks++;

                } else if (text[i] == '\r') {

                    if (i + 1 == text.Length || text[i + 1] != '\n') {

                        lineBreaks++;
                    }
                }
            }

            return lineBreaks;
        }

        public static bool ContainsTab(this string text) {
            return text.Any(ch => ch == '\t');

        }

        public static string CreateIndentString(this int desiredIndentation, int tabSize, bool useTabs) {

            int numberOfTabs   = 0;
            int numberOfSpaces = Math.Max(0, desiredIndentation);

            if (useTabs) {
                numberOfTabs   =  desiredIndentation / tabSize;
                numberOfSpaces -= numberOfTabs       * tabSize;
            }

            return new string('\t', numberOfTabs) + new string(' ', numberOfSpaces);
        }

        public const string NewLine = "\r\n";

        public static string ReindentMultilineComment(this string comment, int desiredIndentation, int tabSize, bool useTabs) {

            var trimChars = new[] {'\r', '\n'};

            var sb    = new StringBuilder();
            var lines = comment.Split('\n');

            //var separator = "";
            for (int i = 0; i < lines.Length; ++i) {

                var line = lines[i].Trim(trimChars);

                var nonWhitespaceindex = line.GetFirstNonWhitespaceIndex();
                if (nonWhitespaceindex >= 0) {

                    var text = line.Substring(nonWhitespaceindex, line.Length - nonWhitespaceindex);
                    var column = line.GetColumnFromPosition(tabSize, nonWhitespaceindex);

                    var indentString = CreateIndentString(desiredIndentation+column, tabSize, useTabs);

                    sb.Append(indentString).Append(text);
                }

                if (i < lines.Length - 1) {
                    sb.Append(NewLine);
                }

            }

            return sb.ToString();

        }

    }

}