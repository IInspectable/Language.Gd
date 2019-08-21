#region Using Directives

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public readonly struct SyntaxToken {

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

        // TODO Leading / Trailing Trivias
        public string Text => SyntaxTree.SourceText.Substring(Extent);

        public bool HasLeadingTrivia   => Slot.LeadingTrivias.Length  > 0;
        public bool HasTrailingTrivias => Slot.TrailingTrivias.Length > 0;

        public int        ExtentStart => Position + Slot.GetLeadingTriviaWidth();
        public TextExtent Extent      => Slot.GetExtent(Position);
        public TextExtent FullExtent  => TextExtent.FromBounds(start: Position, end: EndPosition);

        internal int Position    { get; }
        internal int EndPosition => Position + Slot.FullLength;

        internal int Length     => Slot.Length;
        internal int FullLength => Slot.FullLength;

        public override string ToString() {
            return $"{Kind}: {SyntaxTree.SourceText.Substring(Extent)}";
        }

    }

}