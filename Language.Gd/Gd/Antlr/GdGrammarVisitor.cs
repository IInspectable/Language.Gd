using System.Linq;
using System.Collections.Immutable;

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

using Pharmatechnik.Language.Text;
using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd.Antlr {

   
    partial class GdSyntaxSlotBuilder: GdGrammarBaseVisitor<SyntaxSlot> {

        private readonly ImmutableArray<TokenSlot> _tokens;

        public GdSyntaxSlotBuilder(ImmutableArray<TokenSlot> tokens) {
            _tokens = tokens;

        }

        //public override SyntaxSlot VisitTemplate([NotNull] GdGrammar.TemplateContext context) {

        //    return base.VisitTemplate(context);
        //}

        //public override SyntaxSlot VisitQualifiedName([NotNull] GdGrammar.QualifiedNameContext context) {

        //    return new QualifiedNameSyntaxSlot(
        //        GetExtent(context),
        //        context.identifierName()
        //               .Required(VisitIdentifierName)
        //               .OfType<IdentifierNameSyntaxSlot>(),
        //        context.qualifiedNameContinuation()
        //               .Optional(VisitQualifiedNameContinuation)
        //               .OfType<QualifiedNameContinuationSyntaxSlot>()
        //    );
        //}

        //public override SyntaxSlot VisitIdentifierName([NotNull] GdGrammar.IdentifierNameContext context) {
        //    return new IdentifierNameSyntaxSlot(
        //        GetExtent(context),
        //        GetTokenSlot(context.Identifier().Symbol)
        //    );
        //}

        //public override SyntaxSlot VisitPropertiesSection([NotNull] GdGrammar.PropertiesSectionContext context) {
        //    return new PropertiesSectionSyntaxSlot(
        //        GetExtent(context),
        //        context.propertiesSectionBegin()
        //               .Required(VisitPropertiesSectionBegin)
        //               .OfType<PropertiesSectionBeginSyntaxSlot>(),
        //        context.property()
        //               .ZeroOrMore(VisitProperty)
        //               .OfType<PropertySyntaxSlot>(),
        //        context.propertiesSectionEnd()
        //               .Required(VisitPropertiesSectionEnd)
        //               .OfType<PropertiesSectionEndSyntaxSlot>()
        //    );
        //}

        TextExtent GetExtent(ParserRuleContext context) {
            return TextExtentFactory.CreateExtent(context);
        }

        // TODO Perf Optimierung
        TokenSlot GetTokenSlot(IToken terminalNode) {
            // TODO Missing Tokens klären...
            if (terminalNode.StartIndex == -1 || terminalNode.StartIndex == -1) {
                return TokenSlot.Create(TextExtent.Missing, (SyntaxKind) terminalNode.Type);
            }
            return _tokens.First(t => t.Start == terminalNode.StartIndex);
        }

    }

}