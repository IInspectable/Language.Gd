using System;

namespace Pharmatechnik.Language.Text {

    [Serializable]
    public readonly struct LinePosition: IEquatable<LinePosition>, IComparable<LinePosition> {

        readonly int _line;
        readonly int _character;

        public LinePosition(int line, int character) {

            if (line < 0) {
                throw new ArgumentOutOfRangeException(nameof(line));
            }

            if (character < 0) {
                throw new ArgumentOutOfRangeException(nameof(character));
            }

            _line      = line;
            _character = character;
        }

        // TODO Missing hinzufügen
        public static readonly LinePosition Empty = new LinePosition(0, 0);

        /// <summary>
        /// The line number. The first line in a file is defined as line 0 (zero based line numbering).
        /// </summary>
        public int Line => _line;

        /// <summary>
        /// The character position within the line (zero based).
        /// </summary>
        public int Character => _character;

        /// <summary>
        /// Determines whether two <see cref="LinePosition"/> are the same.
        /// </summary>
        public static bool operator ==(LinePosition left, LinePosition right) {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="LinePosition"/> are different.
        /// </summary>
        public static bool operator !=(LinePosition left, LinePosition right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="LinePosition"/> are the same.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        public bool Equals(LinePosition other) {
            return other.Line == Line && other.Character == Character;
        }

        /// <summary>
        /// Determines whether two <see cref="LinePosition"/> are the same.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        public override bool Equals(object obj) {
            return obj is LinePosition position && Equals(position);
        }

        /// <summary>
        /// Provides a hash function for <see cref="LinePosition"/>.
        /// </summary>
        public override int GetHashCode() {
            return Line ^ Character;
        }

        /// <summary>
        /// Provides a string representation for <see cref="LinePosition"/>.
        /// For better readability line numbers and characters are converted to 1 based numbers.
        /// </summary>
        /// <example>1,5</example>
        public override string ToString() {
            return $"{Line + 1},{Character + 1}";
        }

        public int CompareTo(LinePosition other) {
            int result = _line.CompareTo(other._line);
            return result != 0 ? result : _character.CompareTo(other.Character);
        }

        public static bool operator >(LinePosition left, LinePosition right) {
            return left.CompareTo(right) > 0;
        }

        public static bool operator >=(LinePosition left, LinePosition right) {
            return left.CompareTo(right) >= 0;
        }

        public static bool operator <(LinePosition left, LinePosition right) {
            return left.CompareTo(right) < 0;
        }

        public static bool operator <=(LinePosition left, LinePosition right) {
            return left.CompareTo(right) <= 0;
        }

    }

}