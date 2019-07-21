using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using Pharmatechnik.Language.Gd.Antlr;

namespace Pharmatechnik.Language.Gd.Internal {

    class TokenFactory {

        public static ImmutableArray<TokenSlot> CreateTokens(GdCommonTokenStream tokenstream, List<IErrorNode> errorNodes) {

            var rawTokens     = tokenstream.AllTokens;
            var tokens        = ImmutableArray.CreateBuilder<TokenSlot>(rawTokens.Count);
            var skippedTokens = new List<IToken>(errorNodes.Select(n => n.Symbol));

            var tokenBuilder = new TokenBuilder();

            foreach (var rawToken in rawTokens) {

                var kind                = GetTokenKind(rawToken);
                var extent              = TextExtentFactory.CreateExtent(rawToken);
                var isSkipedTokenTrivia = skippedTokens.Contains(rawToken);

                if (rawToken.Channel == GdTokens.TriviaChannel ||
                    rawToken.Channel == GdTokens.Hidden        ||
                    isSkipedTokenTrivia) {

                    var trivia = TriviaSlot.Create(extent, kind, isSkipedTokenTrivia);
                    tokenBuilder.AddTrivia(trivia);
                } else {

                    tokenBuilder.AddToken(extent, kind, out var completedToken);
                    if (completedToken != null) {
                        tokens.Add(completedToken);
                    }
                }

            }

            var lastToken = tokenBuilder.TryCompleteToken();
            if (lastToken != null) {
                tokens.Add(lastToken);
            }

            return tokens.ToImmutable();
        }

        static SyntaxKind GetTokenKind(IToken token) {
            return (SyntaxKind) token.Type;
        }

    }

}