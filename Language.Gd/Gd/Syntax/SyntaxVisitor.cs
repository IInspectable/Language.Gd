namespace Pharmatechnik.Language.Gd {

    public partial class SyntaxVisitor {

        public virtual void Visit(SyntaxNode node) {

            node?.Accept(this);
        }

        protected virtual void DefaultVisit(SyntaxNode node) {
        }

    }

    public partial class SyntaxVisitor<TResult> {

        public virtual TResult Visit(SyntaxNode node) {
            if (node == null) {
                return default;
            }

            return node.Accept(this);
        }

        protected virtual TResult DefaultVisit(SyntaxNode node) {
            return default;
        }

    }

}