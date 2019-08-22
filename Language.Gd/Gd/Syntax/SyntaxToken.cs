#region Using Directives

using System;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public readonly struct SyntaxToken: IEquatable<SyntaxToken> {

        internal SyntaxToken(SyntaxTree syntaxTree, TokenSlot slot, SyntaxNode parent, int position) {
            Position   = position;
            SyntaxTree = syntaxTree;
            Parent     = parent;
            Slot       = slot;

        }

        internal TokenSlot Slot { get; }

        public SyntaxTree SyntaxTree { get; }
        public SyntaxNode Parent     { get; }

        public SyntaxKind Kind => Slot.Kind;

        public bool IsMissing => Slot.IsMissing;
        public bool IsKeyword => SyntaxFacts.IsKeyword(Kind);

        public string Text => SyntaxTree.SourceText.Substring(Extent);

        // TODO Leading / Trailing Trivias
        public bool HasLeadingTrivia  => Slot.LeadingTrivia.Length  > 0;
        public bool HasTrailingTrivia => Slot.TrailingTrivia.Length > 0;

        public SyntaxTriviaList LeadingTrivia  => new SyntaxTriviaList(this, Slot.LeadingTrivia,  Position);
        public SyntaxTriviaList TrailingTrivia => new SyntaxTriviaList(this, Slot.TrailingTrivia, Position + FullLength - Slot.GetTrailingTriviaWidth());

        public int        ExtentStart => Position + Slot.GetLeadingTriviaWidth();
        public TextExtent Extent      => Slot.GetExtent(Position);
        public TextExtent FullExtent  => TextExtent.FromBounds(start: Position, end: EndPosition);

        internal int Position    { get; }
        internal int EndPosition => Position + Slot.FullLength;

        internal int Length     => Slot.Length;
        internal int FullLength => Slot.FullLength;

        public override string ToString() {
            return $"T: {Kind}: {SyntaxTree.SourceText.Substring(Extent)}";
        }

        public bool Equals(SyntaxToken other) {
            return Slot       == other.Slot       &&
                   SyntaxTree == other.SyntaxTree &&
                   Parent     == other.Parent     &&
                   Position   == other.Position;
        }

        public override bool Equals(object obj) {
            return obj is SyntaxToken other &&
                   Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (Slot != null ? Slot.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SyntaxTree != null ? SyntaxTree.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Parent     != null ? Parent.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Position;
                return hashCode;
            }
        }

        public static bool operator ==(SyntaxToken left, SyntaxToken right) {
            return left.Equals(right);
        }

        public static bool operator !=(SyntaxToken left, SyntaxToken right) {
            return !left.Equals(right);
        }

    }

}