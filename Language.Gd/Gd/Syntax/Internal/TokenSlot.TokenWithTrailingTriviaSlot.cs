using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        private class TokenWithTrailingTriviaSlot: TokenSlot {

            public TokenWithTrailingTriviaSlot(int fullLength, SyntaxKind kind, ImmutableArray<TriviaSlot> trailingTrivias)
                : base(fullLength, kind) {
                TrailingTrivias = trailingTrivias;
            }

            public override ImmutableArray<TriviaSlot> TrailingTrivias { get; }

        }

    }

}