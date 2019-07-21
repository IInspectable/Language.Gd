using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    abstract class SyntaxSlot: Slot {

        protected SyntaxSlot(TextExtent textExtent, SyntaxKind kind)
            : base(textExtent, kind) {
        }

        public abstract SyntaxNode Realize(
            SyntaxTree syntaxTree,
            SyntaxNode parent);

    }

}