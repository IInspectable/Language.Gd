using System.Collections.Immutable;

using Antlr4.Runtime;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd.Antlr {

    partial class GdSyntaxSlotBuilder {

        private readonly ImmutableDictionary<int, TokenSlot> _tokens;

        public GdSyntaxSlotBuilder(ImmutableDictionary<int, TokenSlot> tokens) {
            _tokens = tokens;

        }

        TokenSlot GetTokenSlot([CanBeNull] IToken terminalNode, SyntaxKind syntaxKind) {

            var extent = TextExtentFactory.CreateExtent(terminalNode);
            if (extent.IsMissing) {
                return TokenSlot.Create(length: 0, kind: syntaxKind);
            }

            return _tokens[extent.Start];
        }

    }

}