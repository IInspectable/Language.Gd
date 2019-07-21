using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    public class TriviaSlot: Slot {

        TriviaSlot(TextExtent textExtent, SyntaxKind kind)
            : base(textExtent, kind) {
        }

        internal static TriviaSlot Create(TextExtent textExtent, SyntaxKind kind) {
            return new TriviaSlot(textExtent, kind);
        }

        internal SyntaxTrivia Realize(SyntaxTree syntaxTree, SyntaxToken token) {
            return new SyntaxTrivia(syntaxTree, token, this);
        }

    }

}