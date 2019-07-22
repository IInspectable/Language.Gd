using System.Linq;
using System.Collections.Immutable;

using Antlr4.Runtime;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;
using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd.Antlr {

    partial class GdSyntaxSlotBuilder: GdGrammarBaseVisitor<SyntaxSlot> {

        private readonly ImmutableArray<TokenSlot> _tokens;

        public GdSyntaxSlotBuilder(ImmutableArray<TokenSlot> tokens) {
            _tokens = tokens;

        }

        TextExtent GetExtent(ParserRuleContext context) {
            return TextExtentFactory.CreateExtent(context);
        }

        // TODO Perf Optimierung /OptinalTokens
        TokenSlot GetTokenSlot([CanBeNull] IToken terminalNode, SyntaxKind syntaxKind) {
            // TODO Missing Tokens klären...

            if (terminalNode == null) {
                return TokenSlot.Create(TextExtent.Missing, syntaxKind);
            }

            //if (terminalNode.StartIndex == -1 || terminalNode.StartIndex == -1) {

            //    return TokenSlot.Create(TextExtent.Missing, syntaxKind);
            //}

            return _tokens.First(t => t.Start == terminalNode.StartIndex);
        }

    }

}