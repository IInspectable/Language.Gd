#region Using Directives

using System;
using System.Collections.Immutable;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Language.Text {

    public static class StringExtensions {

        [ContractAnnotation("null=>false")]
        public static bool IsEmpty([NotNull] this string value) {
            return value.Length == 0;
        }

        [ContractAnnotation("null=>true")]
        public static bool IsNullOrEmpty([CanBeNull] this string value) {
            return String.IsNullOrEmpty(value);
        }

        [ContractAnnotation("null=>true")]
        public static bool IsNullOrWhiteSpace([CanBeNull] this string value) {
            return String.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Liefert null, wenn die angegebene Zeichenfolge null oder String.Empty ist;
        /// andernfalls wird die angegebene Zeichenfolge zurückgegeben.
        /// </summary>
        [CanBeNull]
        public static string NullIfEmpty([CanBeNull] this string value) {
            return value.IsNullOrEmpty() ? null : value;
        }

        /// <summary>
        /// Liefert null, wenn die angegebene Zeichenfolge null oder String.Empty ist, oder nur aus Whitespaces besteht;
        /// andernfalls wird die angegebene Zeichenfolge zurückgegeben.
        /// </summary>
        [CanBeNull]
        public static string NullIfWhiteSpace([CanBeNull] this string value) {
            return value.IsNullOrWhiteSpace() ? null : value;
        }

        public static string Substring(this string text, TextExtent extent) {
            if (extent.IsMissing) {
                return String.Empty;
            }

            return text.Substring(startIndex: extent.Start, length: extent.Length);
        }

        public static ReadOnlySpan<char> Slice(this ReadOnlySpan<char> span, TextExtent extent) {
            if (extent.IsMissing) {
                return ReadOnlySpan<char>.Empty;
            }

            return span.Slice(start: extent.Start, length: extent.Length);
        }

        public static int IndexOfPreviousNonWhitespace(this string text, int position) {
            return IndexOfPreviousNonWhitespace(text.AsSpan(), position);
        }

        public static int IndexOfPreviousNonWhitespace(this ReadOnlySpan<char> span, int position) {

            if (position == 0) {
                return -1;
            }

            do {
                position -= 1;
            } while (position > 0 && char.IsWhiteSpace(span[position]));

            return position;
        }

        public static bool IsInTextBlock(this string text, int position, char blockStartChar, char blockEndChar) {
            return text.AsSpan().IsInTextBlock(position, blockStartChar, blockEndChar);
        }

        public static bool IsInTextBlock(this ReadOnlySpan<char> text, int position, char blockStartChar, char blockEndChar) {
            return IsInTextBlockImpl(text, position, blockStartChar, blockEndChar, out _);
        }

        public static bool IsInQuotation(this string text, int position, char quotationChar = '"') {
            return text.AsSpan().IsInQuotation(position, quotationChar);
        }

        public static bool IsInQuotation(this ReadOnlySpan<char> text, int position, char quotationChar = '"') {
            return IsInTextBlockImpl(text, position, quotationChar, out _);
        }

        /// <summary>
        /// Liefert den gequoteten Bereich um position. Wenn der Bereich nach hinten offen ist, d.h. nicht
        /// explizit mit dem angegebenen quotationChar abschließt, hört der Bereich mit dem ersten
        /// Whitespace nach position auf.
        /// Gibt es weder ein abschließendes quotationChar noch ein terminierendes Whitespace,
        /// wird der Bereich vom Anfang der quitierung bis zum Ende der angegebenen Zeichenfolge
        /// zurückgeliefert
        /// </summary>
        public static TextExtent QuotedExtent(this string text, int position, char quotationChar = '"', bool includequotationCharInExtent = false) {
            return text.AsSpan().QuotedExtent(position, quotationChar, includequotationCharInExtent);
        }

        public static TextExtent QuotedExtent(this ReadOnlySpan<char> text, int position, char quotationChar = '"', bool includequotationCharInExtent = false) {

            if (!IsInTextBlockImpl(text, position, quotationChar, out var start)) {
                return TextExtent.Missing;
            }

            int offset = 0;
            if (includequotationCharInExtent) {
                offset = 1;
            }

            start++;
            int firstWhiteSpace = -1;
            for (int index = start; index < text.Length; index++) {
                if (text[index] == quotationChar) {

                    return TextExtent.FromBounds(start: start - offset, end: index + offset);
                }

                if (firstWhiteSpace == -1 && Char.IsWhiteSpace(text[index])) {
                    firstWhiteSpace = index;
                }
            }

            if (firstWhiteSpace != -1) {
                return TextExtent.FromBounds(start: start - offset, end: firstWhiteSpace);
            }

            return TextExtent.FromBounds(start: start - offset, end: text.Length);
        }

        static bool IsInTextBlockImpl(this ReadOnlySpan<char> text, int position, char quotationChar, out int quotationStart) {

            quotationStart = -1;

            if (position < 0 || position > text.Length) {
                return false;
            }

            bool inQuotation = false;
            for (int index = 0; index < position; index++) {
                if (text[index] == quotationChar) {

                    inQuotation ^= true;
                    if (inQuotation) {
                        quotationStart = index;
                    }
                }

            }

            return inQuotation;
        }

        static bool IsInTextBlockImpl(this ReadOnlySpan<char> text, int position, char blockStartChar, char blockEndChar, out int quotationStart) {

            if (blockStartChar == blockEndChar) {
                return IsInTextBlockImpl(text, position, blockStartChar, out quotationStart);
            }

            quotationStart = -1;

            if (position < 0 || position > text.Length) {
                return false;
            }

            // TODO Was ist mit nested Blocks?

            int blockEntered = 0;
            for (int index = 0; index < position; index++) {

                if (text[index] == blockStartChar) {
                    blockEntered++;
                    quotationStart = index;
                } else if (text[index] == blockEndChar && blockEntered > 0) {
                    blockEntered--;
                }

            }

            return blockEntered > 0;
        }

        [NotNull]
        public static string ToCamelcase(this string s) {

            if (String.IsNullOrEmpty(s)) {
                return s ?? String.Empty;
            }

            return s.Substring(0, 1).ToLowerInvariant() + s.Substring(1);
        }

        [NotNull]
        public static string ToPascalcase(this string s) {

            if (String.IsNullOrEmpty(s)) {
                return s ?? String.Empty;
            }

            return s.Substring(0, 1).ToUpperInvariant() + s.Substring(1);
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
        public static int GetColumnForOffset(this ReadOnlySpan<char> text, int tabSize, int offset) {
            var column = 0;
            for (int index = 0; index < offset; index++) {
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
        public static int GetIndentationColumn(this ReadOnlySpan<char> text, int tabSize) {
            return GetIndentationColumn(text, tabSize, out _);
        }

        /// <summary>
        /// Liefert den Spaltenindex (beginnend bei 0) für das erste signifikante Zeichen in der angegebenen Zeile.
        /// Als nicht signifikant gelten alle Arten von Leerzeichen. Dabei werden Tabulatoren entsprechend umgerechnet.
        /// <list type="bullet">
        /// <item>Spaltenindex für das erste signifikante Zeichen</item>
        /// <item><see cref="int.MaxValue"/>, wenn es kein signifikantes Zeichen gibt</item>
        /// </list>
        /// <para>See also: <see cref="StringExtensions.GetIndentationColumn(ReadOnlySpan&lt;char&gt;, int)"/></para>
        /// </summary>
        public static int GetIndentationColumn(this ReadOnlySpan<char> text, int tabSize, out int index) {
            bool hasSignificantContent = false;
            int  column                = 0;
            for (index = 0; index < text.Length; index++) {
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

            if (!hasSignificantContent) {
                //   index = -1;
                return Int32.MaxValue;
            }

            return column;
        }

        public static ImmutableArray<int> ParseLineStarts(this ReadOnlySpan<char> text) {

            if (text.Length == 0) {
                return ImmutableArray.Create(0);
            }

            var lineStarts = ImmutableArray.CreateBuilder<int>();

            int index;
            int lineStart = 0;
            for (index = 0; index < text.Length; index++) {

                char c = text[index];

                bool isNewLine = false;

                if (c == '\n') {
                    isNewLine = true;
                } else if (c == '\r') {
                    isNewLine = true;
                    // => \r\n
                    if (index + 1 < text.Length && text[index + 1] == '\n') {
                        index++;
                    }
                }

                if (isNewLine) {
                    // Achtung: Extent End zeigt immer _hinter_ das letzte Zeichen!
                    var lineEnd = index + 1;
                    lineStarts.Add(lineStart);
                    lineStart = lineEnd;
                }
            }

            // Einzige/letzte Zeile nicht vergessen. 
            if (index >= lineStart) {
                lineStarts.Add(lineStart);
            }

            return lineStarts.ToImmutable();
        }

        /// <summary>
        /// Liefert die Anzahl an Zeichen des Zeilenvorschubs, oder 0, falls der Text nicht mit einem Zeilenvorschub endet.
        /// </summary>
        public static int GetNewLineCharCount(this string text) {
            return GetNewLineCharCount(text.AsSpan());
        }

        /// <summary>
        /// Liefert die Anzahl an Zeichen des Zeilenvorschubs, oder 0, falls der Text nicht mit einem Zeilenvorschub endet.
        /// </summary>
        public static int GetNewLineCharCount(this ReadOnlySpan<char> text) {

            if (text.Length >= 1) {
                if (text[text.Length - 1] == '\n') {

                    if (text.Length >= 2 && text[text.Length - 2] == '\r') {
                        // \r\n
                        return 2;
                    }

                    // \n
                    return 1;
                }

            }

            return 0;
        }

    }

}