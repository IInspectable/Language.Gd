using System.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    abstract class SyntaxSlot: Slot {

        protected SyntaxSlot(SyntaxKind kind)
            : base(kind) {
        }

        public abstract SyntaxNode Realize(
            SyntaxTree syntaxTree,
            SyntaxNode parent,
            int position);

        public override void WriteTo(StringBuilder sb, bool includeLeadingTrivia, bool includeTrailingTrivia) {
            WriteChildSlotsTo(sb, includeLeadingTrivia, includeTrailingTrivia);
        }

    }

}