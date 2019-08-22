using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        private class TokenWithTrailingTriviaSlot: TokenSlot {

            public TokenWithTrailingTriviaSlot(int length, SyntaxKind kind, ImmutableArray<TriviaSlot> trailingTrivia)
                : base(length, kind) {

                TrailingTrivia = trailingTrivia;

                foreach (var t in trailingTrivia) {
                    AdjustLength(t);
                }
            }

            public override ImmutableArray<TriviaSlot> TrailingTrivia { get; }

        }

    }

}