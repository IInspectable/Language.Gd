using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

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

        // TODO
        internal int Position { get; }

        //public TextExtent Extent      => _slot.Extent;
        //public int        ExtentEnd   => _slot.End;

        public string Text => SyntaxTree.SourceText.Substring(Extent);

        public bool HasLeadingTrivia   => Slot.LeadingTrivias.Length  > 0;
        public bool HasTrailingTrivias => Slot.TrailingTrivias.Length > 0;

        public TextExtent Extent {
            get {
                // Start with the full span.
                var start  = Position;
                var length = Slot.FullLength;

                // adjust for preceding trivia (avoid calling this twice, do not call Green.Width)
                var precedingWidth = Slot.GetLeadingTriviaWidth();
                start  += precedingWidth;
                length -= precedingWidth;

                // adjust for following trivia width
                length -= Slot.GetTrailingTriviaWidth();

                return new TextExtent(start, length);
            }
        }

        public override string ToString() {
            return $"{Kind}: {SyntaxTree.SourceText.Substring(Extent)}" ;
        }

    }

}