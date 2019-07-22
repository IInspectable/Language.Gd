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
        TokenSlot GetTokenSlot([CanBeNull] IToken terminalNode) {
            // TODO Missing Tokens klären...
            if (terminalNode == null || terminalNode.StartIndex == -1 || terminalNode.StartIndex == -1) {

                var type = SyntaxKind.MinusAlt;
                if (terminalNode != null) {
                    type = (SyntaxKind) terminalNode.Type;
                }

                return TokenSlot.Create(TextExtent.Missing, type);
            }

            return _tokens.First(t => t.Start == terminalNode.StartIndex);
        }

    }

}