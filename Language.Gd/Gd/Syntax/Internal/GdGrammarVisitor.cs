using System.Collections.Generic;
using System.Collections.Immutable;

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Pharmatechnik.Language.Gd.Antlr;

namespace Pharmatechnik.Language.Gd.Internal {

    class ErrorNodeVisitor: GdGrammarBaseVisitor<bool> {

       
        public List<IErrorNode> ErrorNodes { get; } = new List<IErrorNode>();

        public override bool VisitErrorNode([NotNull] IErrorNode node) {
            ErrorNodes.Add(node);
            return base.VisitErrorNode(node);
        }

    }

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