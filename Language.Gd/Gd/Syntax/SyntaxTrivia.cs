#region Using Directives

using System;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public readonly struct SyntaxTrivia: IEquatable<SyntaxTrivia> {

        internal SyntaxTrivia(SyntaxToken token, TriviaSlot slot, int position) {
            Position = position;
            Token    = token;
            Slot     = slot;
        }

        [CanBeNull]
        public SyntaxTree SyntaxTree => Token.SyntaxTree;

        public SyntaxToken Token { get; }

        [CanBeNull]
        internal TriviaSlot Slot { get; }

        public SyntaxKind Kind => Slot?.Kind ?? SyntaxKind.None;

        public bool IsSkipedTokenTrivia => Slot?.IsSkipedTokenTrivia ?? false;

        public int        ExtentStart => Position + Slot?.GetLeadingTriviaWidth() ?? default;
        public TextExtent Extent      => Slot != null ? new TextExtent(start: Position, length: Slot.FullLength) : default;

        internal int Position    { get; }
        internal int EndPosition => Position + Slot?.FullLength ?? 0;

        public int    Length => Slot?.Length ?? 0;
        public string Text   => Slot?.Text   ?? String.Empty;

        public Location GetLocation() {
            return SyntaxTree?.GetLocation(Extent) ?? Location.None;
        }

        public override string ToString() {
            return $"Tr: {Kind}: {Text}";
        }

        public bool Equals(SyntaxTrivia other) {
            return Equals(Slot,       other.Slot)       &&
                   Equals(SyntaxTree, other.SyntaxTree) &&
                   Token.Equals(other.Token)            &&
                   Position == other.Position;
        }

        public override bool Equals(object obj) {
            return obj is SyntaxTrivia other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (Slot != null ? Slot.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SyntaxTree != null ? SyntaxTree.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Token.GetHashCode();
                hashCode = (hashCode * 397) ^ Position;
                return hashCode;
            }
        }

        public static bool operator ==(SyntaxTrivia left, SyntaxTrivia right) {
            return left.Equals(right);
        }

        public static bool operator !=(SyntaxTrivia left, SyntaxTrivia right) {
            return !left.Equals(right);
        }

    }

}