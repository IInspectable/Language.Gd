#region Using Directives

using System;

#endregion

namespace Pharmatechnik.Language.Text {

    public readonly struct TextChange: IEquatable<TextChange> {

        private readonly string _replacementText;

        TextChange(TextExtent extent, string replacementText) {
            Extent           = extent;
            _replacementText = replacementText ?? throw new ArgumentNullException(nameof(replacementText));
        }

        public static TextChange NewInsert(int position, string text) {
            return new TextChange(TextExtent.FromBounds(position, position), text);
        }

        public static TextChange NewRemove(int start, int length) {
            return NewRemove(new TextExtent(start, length));
        }

        public static TextChange NewRemove(TextExtent extent) {
            return new TextChange(extent, String.Empty);
        }

        public static TextChange NewReplace(int start, int length, string text) {
            return NewReplace(new TextExtent(start, length), text);
        }

        public static TextChange NewReplace(TextExtent extent, string text) {
            return new TextChange(extent, text);
        }

        public static readonly TextChange Empty = new TextChange();

        public TextExtent Extent { get; }

        public string ReplacementText => _replacementText ?? String.Empty;

        public bool IsEmpty => Extent.IsEmpty && ReplacementText == String.Empty;

        public override string ToString() {
            return $"{GetType().Name}: {{ {Extent}, \"{ReplacementText}\" }}";
        }

        #region Equality members

        public bool Equals(TextChange other) {
            return Extent.Equals(other.Extent) && string.Equals(ReplacementText, other.ReplacementText);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;

            return obj is TextChange change && Equals(change);
        }

        public override int GetHashCode() {
            unchecked {
                return (Extent.GetHashCode() * 397) ^ ReplacementText.GetHashCode();
            }
        }

        public static bool operator ==(TextChange left, TextChange right) {
            return left.Equals(right);
        }

        public static bool operator !=(TextChange left, TextChange right) {
            return !left.Equals(right);
        }

        #endregion

    }

}