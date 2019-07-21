using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd {

    public readonly struct SyntaxTrivia {

        private readonly TriviaSlot _slot;

        internal SyntaxTrivia(SyntaxTree syntaxTree, SyntaxToken token, TriviaSlot slot) {
            SyntaxTree = syntaxTree;
            Token      = token;
            _slot      = slot;
        }

        public SyntaxTree  SyntaxTree { get; }
        public SyntaxToken Token      { get; }

        public SyntaxKind Kind => _slot.Kind;

        public bool IsMissing           => _slot.IsMissing;
        public bool IsSkipedTokenTrivia => _slot.IsSkipedTokenTrivia;

        public int        ExtentStart => _slot.Start;
        public TextExtent Extent      => _slot.Extent;
        public int        ExtentEnd   => _slot.End;

        public string Text => _slot.ToString();

    }

}