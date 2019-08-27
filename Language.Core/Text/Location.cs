#region Using Directives

using System;

using JetBrains.Annotations;

using Pharmatechnik.Language.IO;

#endregion

namespace Pharmatechnik.Language.Text {

    [Serializable]
    public class Location: IEquatable<Location> {

        private Location() {

        }

        protected Location(Location location) {
            Extent            = location.Extent;
            StartLinePosition = location.StartLinePosition;
            EndLinePosition   = location.EndLinePosition;
            FilePath          = location.FilePath;
        }

        public Location(TextExtent extent, LineRange lineRange, [CanBeNull] string filePath) {
            Extent            = extent;
            StartLinePosition = lineRange.Start;
            EndLinePosition   = lineRange.End;
            FilePath          = filePath;
        }

        public Location(TextExtent extent, LinePosition linePosition, [CanBeNull] string filePath):
            this(extent, new LineRange(linePosition, linePosition), filePath) {
        }

        public Location(string filePath) {
            Extent            = TextExtent.Empty;
            StartLinePosition = LinePosition.Empty;
            EndLinePosition   = LinePosition.Empty;
            FilePath          = filePath;
        }

        public static readonly Location None = new Location();

        public TextExtent   Extent            { get; }
        public LinePosition StartLinePosition { get; }
        public LinePosition EndLinePosition   { get; }
        public LineRange    LineRange         => new LineRange(StartLinePosition, EndLinePosition);

        /// <summary>
        /// The path to the file or null.
        /// </summary>
        [CanBeNull]
        public string FilePath { get; }

        [CanBeNull]
        public string NormalizedFilePath => PathHelper.NormalizePath(FilePath);

        /// <summary>
        /// Gets the starting index of the location [0..n].
        /// -1 if the is unknown/missing
        /// </summary>
        public int Start => Extent.Start;

        /// <summary>
        /// The start line number of the location. The first line in a file is defined as line 0 (zero based line numbering).
        /// </summary>
        public int StartLine => StartLinePosition.Line;

        /// <summary>
        /// The character position within the starting line (zero based).
        /// </summary>
        public int StartCharacter => StartLinePosition.Character;

        /// <summary>
        /// Gets the length of the location. Length is guaranteed to be great or equal to 0.
        /// </summary>
        public int Length => Extent.Length;

        /// <summary>
        /// Gets the end index of the location, starting with 0;
        /// This index is actually one character past the end of the location.
        /// </summary>
        public int End => Extent.End;

        /// <summary>
        /// The end line number of the location. The first line in a file is defined as line 0 (zero based line numbering).
        /// </summary>
        public int EndLine => EndLinePosition.Line;

        /// <summary>
        /// The character position within the end line (zero based).
        /// </summary>
        public int EndCharacter => EndLinePosition.Character;

        public override string ToString() {
            return $"{FilePath}@{StartLine + 1}:{StartCharacter + 1}";
        }

        #region Equality members

        public bool Equals(Location other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Extent.Equals(other.Extent)                       &&
                   StartLinePosition.Equals(other.StartLinePosition) &&
                   EndLinePosition.Equals(other.EndLinePosition)     &&
                   string.Equals(NormalizedFilePath, other.NormalizedFilePath);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            return obj is Location location && Equals(location);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = Extent.GetHashCode();
                hashCode = (hashCode * 397) ^ StartLinePosition.GetHashCode();
                hashCode = (hashCode * 397) ^ EndLinePosition.GetHashCode();
                hashCode = (hashCode * 397) ^ (NormalizedFilePath?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static bool operator ==(Location left, Location right) {
            return Equals(left, right);
        }

        public static bool operator !=(Location left, Location right) {
            return !Equals(left, right);
        }

        #endregion

    }

}