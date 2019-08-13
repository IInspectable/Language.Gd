using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd {

    public readonly struct SyntaxToken {

        private readonly TokenSlot _slot;

        internal SyntaxToken(SyntaxTree syntaxTree, SyntaxNode parent, TokenSlot slot) {
            Start = 0; // TODO Start
            SyntaxTree = syntaxTree;
            Parent     = parent;
            _slot      = slot;

        }

        public SyntaxTree SyntaxTree { get; }
        public SyntaxNode Parent     { get; }

        public SyntaxKind Kind => _slot.Kind;

        public bool IsMissing => _slot.IsMissing;

        // TODO
        public int Start { get; }
        //public TextExtent Extent      => _slot.Extent;
        //public int        ExtentEnd   => _slot.End;

        public string Text => _slot.ToString();

        public bool HasLeadingTrivia   => _slot.LeadingTrivias.Length  > 0;
        public bool HasTrailingTrivias => _slot.TrailingTrivias.Length > 0;

    }

}