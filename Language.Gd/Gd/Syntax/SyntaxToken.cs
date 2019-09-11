#region Using Directives

using System;
using System.Collections.Immutable;

using JetBrains.Annotations;

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

        [CanBeNull]
        internal TokenSlot Slot { get; }

        [CanBeNull]
        public SyntaxTree SyntaxTree { get; }

        [CanBeNull]
        public SyntaxNode Parent { get; }

        public SyntaxKind Kind => Slot?.Kind ?? SyntaxKind.None;

        public bool IsMissing => Slot?.IsMissing ?? false;
        public bool IsKeyword => SyntaxFacts.IsKeyword(Kind);

        public string Text => SyntaxTree?.SourceText.Substring(Extent) ?? String.Empty;

        public bool HasLeadingTrivia  => Slot?.LeadingTrivia.Length  > 0;
        public bool HasTrailingTrivia => Slot?.TrailingTrivia.Length > 0;

        public SyntaxTriviaList LeadingTrivia  => new SyntaxTriviaList(this, Slot?.LeadingTrivia,  Position);
        public SyntaxTriviaList TrailingTrivia => new SyntaxTriviaList(this, Slot?.TrailingTrivia, Position + FullLength - TrailingWidth);

        internal int LeadingWidth  => Slot?.GetLeadingTriviaWidth()  ?? 0;
        internal int TrailingWidth => Slot?.GetTrailingTriviaWidth() ?? 0;

        public int        ExtentStart => Position + Slot?.GetLeadingTriviaWidth() ?? 0;
        public TextExtent Extent      => Slot?.GetExtent(Position)                ?? default;
        public TextExtent FullExtent  => TextExtent.FromBounds(start: Position, end: EndPosition);

        internal int Position    { get; }
        internal int EndPosition => Position + Slot?.FullLength ?? 0;

        internal int Length     => Slot?.Length     ?? 0;
        internal int FullLength => Slot?.FullLength ?? 0;

        public override string ToString() {
            return $"T: {Kind}: {Text}";
        }

        public Location GetLocation() {
            return SyntaxTree?.GetLocation(Extent) ?? Location.None;
        }

        public bool Equals(SyntaxToken other) {
            return Equals(Slot,       other.Slot)       &&
                   Equals(SyntaxTree, other.SyntaxTree) &&
                   Equals(Parent,     other.Parent)     &&
                   Position == other.Position;
        }

        public override bool Equals(object obj) {
            return obj is SyntaxToken other && Equals(other);
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

        public ImmutableArray<ClassifiedText> ToSimplifiedText() {
            return ToSimplifiedText(null);
        }

        public ImmutableArray<ClassifiedText> ToSimplifiedText(TextEditorSettings editorSettings) {
            return SimplifiedClassificationBuilder.Classify(this, editorSettings);
        }

    }

}