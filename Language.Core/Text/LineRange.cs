using System;

namespace Pharmatechnik.Language.Text {

    [Serializable]
    public readonly struct LineRange: IEquatable<LineRange> {

        public LineRange(LinePosition start, LinePosition end) {

            if (start > end) {
                throw new ArgumentOutOfRangeException(nameof(start));
            }

            Start = start;
            End   = end;
        }

        public LinePosition Start { get; }
        public LinePosition End   { get; }

        /// <summary>
        /// Determines whether two <see cref="LineRange"/> are the same.
        /// </summary>
        public static bool operator ==(LineRange left, LineRange right) {
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="LineRange"/> are different.
        /// </summary>
        public static bool operator !=(LineRange left, LineRange right) {
            return !left.Equals(right);
        }

        /// <summary>
        /// Determines whether two <see cref="LineRange"/> are the same.
        /// </summary>
        /// <param name="other">The object to compare.</param>
        public bool Equals(LineRange other) {
            return other.Start == Start && other.End == End;
        }

        /// <summary>
        /// Determines whether two <see cref="LineRange"/> are the same.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        public override bool Equals(object obj) {
            return obj is LineRange extent && Equals(extent);
        }

        /// <summary>
        /// Provides a hash function for <see cref="LineRange"/>.
        /// </summary>
        public override int GetHashCode() {
            return Start.GetHashCode() ^ End.GetHashCode();
        }

    }

}