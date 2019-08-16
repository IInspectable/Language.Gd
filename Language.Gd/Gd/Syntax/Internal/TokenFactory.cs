using System.Collections.Generic;
using System.Linq;
using System.Collections.Immutable;

using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

using Pharmatechnik.Language.Gd.Antlr;

namespace Pharmatechnik.Language.Gd.Internal {

    class TokenFactory {

        public static ImmutableDictionary<int, TokenSlot> CreateTokens(GdCommonTokenStream tokenstream, List<IErrorNode> errorNodes) {

            var rawTokens     = tokenstream.AllTokens;
            var tokens        = ImmutableDictionary.CreateBuilder<int, TokenSlot>();
            var skippedTokens = new List<IToken>(errorNodes.Select(n => n.Symbol));

            var tokenBuilder = new TokenBuilder();

            foreach (var rawToken in rawTokens) {

                var kind                = GetTokenKind(rawToken);
                var extent              = TextExtentFactory.CreateExtent(rawToken);
                var isSkipedTokenTrivia = skippedTokens.Contains(rawToken);

                if (rawToken.Channel == GdTokens.TriviaChannel ||
                    rawToken.Channel == Lexer.Hidden        ||
                    isSkipedTokenTrivia) {

                    var trivia = TriviaSlot.Create(extent.Length, kind, isSkipedTokenTrivia);
                    tokenBuilder.AddTrivia(trivia);
                } else {

                    tokenBuilder.AddToken(extent, kind, out var completedTokenInfo);
                    if (completedTokenInfo.Token != null) {
                        tokens.Add(completedTokenInfo.Start, completedTokenInfo.Token);
                    }
                }

            }

            var lastTokenInfo = tokenBuilder.TryCompleteToken();
            if (lastTokenInfo.Token != null) {
                tokens.Add(lastTokenInfo.Start, lastTokenInfo.Token);
            }

            return tokens.ToImmutable();
        }

        static SyntaxKind GetTokenKind(IToken token) {
            return (SyntaxKind) token.Type;
        }

    }

}