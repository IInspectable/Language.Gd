using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    class TriviaSlot: Slot {

        TriviaSlot(TextExtent textExtent, SyntaxKind kind, SlotFlags flags)
            : base(textExtent, kind, flags) {
        }

        internal static TriviaSlot Create(TextExtent textExtent, SyntaxKind kind, bool isSkipedTokenTrivia) {
            var flags = isSkipedTokenTrivia ? SlotFlags.IsSkipedTokenTrivia : SlotFlags.None;
            return new TriviaSlot(textExtent, kind, flags);
        }

        internal SyntaxTrivia Realize(SyntaxTree syntaxTree, SyntaxToken token) {
            return new SyntaxTrivia(syntaxTree, token, this);
        }

    }

    

}