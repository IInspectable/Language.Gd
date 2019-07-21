using System;

using JetBrains.Annotations;

namespace Pharmatechnik.Language.Text {

    /// <summary>
    /// Represents an extent of a single line.
    /// </summary>
    [Serializable]
    public readonly struct SourceTextLine: IExtent, IEquatable<SourceTextLine> {

        internal SourceTextLine(StringSourceText sourceText, int line, int lineStart, int lineEnd) {

            if (sourceText == null) {
                throw new ArgumentNullException(nameof(sourceText));
            }

            if (TextExtent.FromBounds(lineStart, lineEnd).IsMissing) {
                throw new ArgumentOutOfRangeException(nameof(lineEnd));
            }

            if (line < 0) {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            if (line > lineEnd) {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            if (lineEnd > sourceText.Length) {
                throw new ArgumentOutOfRangeException(nameof(lineEnd));
            }

            SourceText = sourceText;
            Line       = line;
            Start      = lineStart;
            End        = lineEnd;
        }

        [NotNull]
        public SourceText SourceText { get; }

        public ReadOnlySpan<char> Span => SourceText.Slice(Extent);

        public ReadOnlySpan<char> Slice(int charPositionInLine, int length) => SourceText.Slice(Start + charPositionInLine, length);

        [NotNull]
        public Location Location => SourceText.GetLocation(Extent);

        [NotNull]
        public Location GetLocation(int charPositionInLine, int length) {
            var start  = Extent.Start + charPositionInLine;
            var extent = new TextExtent(start: start, length: length);
            return SourceText.GetLocation(extent);
        }

        /// <summary>
        /// The line number. The first line in a file is defined as line 0 (zero based line numbering).
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// The extent of the line.
        /// </summary>
        public TextExtent Extent => TextExtent.FromBounds(Start, End);

        /// <summary>
        /// The extent of the line excluding the line ending
        /// </summary>
        public TextExtent ExtentWithoutLineEndings => new TextExtent(Extent.Start, Extent.Length - Span.GetNewLineCharCount());

        public int Start { get; }
        public int End   { get; }

        /// <summary>
        /// Determines whether two <see cref="SourceTextLine"/> are the same.
        /// </summary>
        public static bool operator ==(SourceTextLine left, SourceTextLine right) {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="TextExtent"/> are different.
        /// </summary>
        public static bool operator !=(SourceTextLine left, SourceTextLine right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="SourceTextLine"/> are the same.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        public bool Equals(SourceTextLine other) {
            return other.Line == Line && other.Extent == Extent;
        }

        /// <summary>
        /// Determines whether two <see cref="SourceTextLine"/> are the same.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        public override bool Equals(object obj) {
            return obj is SourceTextLine extent && Equals(extent);
        }

        public override string ToString() {
            return SourceText.Substring(Extent);
        }

        /// <summary>
        /// Provides a hash function for <see cref="SourceTextLine"/>.
        /// </summary>
        public override int GetHashCode() {
            return Line ^ Extent.GetHashCode();
        }

    }

}