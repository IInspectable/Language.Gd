using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    public abstract class SyntaxSlot: Slot {

        protected SyntaxSlot(TextExtent textExtent, SyntaxKind kind)
            : base(textExtent, kind) {
        }

        public abstract SyntaxNode Realize(
            SyntaxTree syntaxTree,
            SyntaxNode parent);

    }

    public abstract class SyntaxListSlot: Slot {

        protected SyntaxListSlot(TextExtent textExtent, SyntaxKind kind): base(textExtent, SyntaxKind.Colon) {
        }

    }

}