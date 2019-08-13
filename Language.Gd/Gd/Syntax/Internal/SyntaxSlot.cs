namespace Pharmatechnik.Language.Gd.Internal {

    abstract class SyntaxSlot: Slot {

        protected SyntaxSlot(SyntaxKind kind)
            : base(kind) {
        }

        public abstract SyntaxNode Realize(
            SyntaxTree syntaxTree,
            SyntaxNode parent,
            int position);

    }

}