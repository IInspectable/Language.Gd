#region Using Directives

using System;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public readonly struct SyntaxTrivia: IEquatable<SyntaxTrivia> {

        [CanBeNull]
        private readonly TriviaSlot _slot;

        internal SyntaxTrivia(SyntaxTree syntaxTree, SyntaxToken token, TriviaSlot slot, int position) {
            Position   = position;
            SyntaxTree = syntaxTree;
            Token      = token;
            _slot      = slot;
        }

        [CanBeNull]
        public SyntaxTree SyntaxTree { get; }

        public SyntaxToken Token { get; }

        public SyntaxKind Kind => _slot?.Kind ?? SyntaxKind.None;

        public bool IsSkipedTokenTrivia => _slot?.IsSkipedTokenTrivia ?? false;

        public int        ExtentStart => Position + _slot?.GetLeadingTriviaWidth() ?? default;
        public TextExtent Extent      => _slot != null ? new TextExtent(start: Position + _slot.GetLeadingTriviaWidth(), length: _slot.Length) : default;
        public TextExtent FullExtent  => _slot != null ? new TextExtent(start: Position,                                 length: _slot.FullLength) : default;

        internal int Position    { get; }
        internal int EndPosition => Position + _slot?.FullLength ?? 0;

        public int Length     => _slot?.Length     ?? 0;
        public int FullLength => _slot?.FullLength ?? 0;

        public string Text => SyntaxTree?.SourceText.Substring(Extent) ?? String.Empty;

        public Location GetLocation() {
            return SyntaxTree?.GetLocation(Extent) ?? Location.None;
        }

        public override string ToString() {
            return $"Tr: {Kind}: {Text}";
        }

        public bool Equals(SyntaxTrivia other) {
            return Equals(_slot,      other._slot)      &&
                   Equals(SyntaxTree, other.SyntaxTree) &&
                   Token.Equals(other.Token)            &&
                   Position == other.Position;
        }

        public override bool Equals(object obj) {
            return obj is SyntaxTrivia other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (_slot != null ? _slot.GetHashCode() : 0);
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