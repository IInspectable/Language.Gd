using System.Collections.Immutable;

using Antlr4.Runtime.Misc;

using Pharmatechnik.Language.Gd.Antlr;

namespace Pharmatechnik.Language.Gd.Internal {

    class GdGrammarVisitor: GdGrammarBaseVisitor<bool> {

        private readonly ImmutableArray<TokenSlot> _tokens;

        public GdGrammarVisitor(ImmutableArray<TokenSlot> tokens) {
            _tokens = tokens;

        }

        public override bool VisitTemplate([NotNull] GdGrammar.TemplateContext context) {

            return base.VisitTemplate(context);
        }

        public override bool VisitQualifiedName([NotNull] GdGrammar.QualifiedNameContext context) {

            return base.VisitQualifiedName(context);
        }

    }

}