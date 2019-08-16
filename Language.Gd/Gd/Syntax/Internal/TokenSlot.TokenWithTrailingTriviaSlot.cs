using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        private class TokenWithTrailingTriviaSlot: TokenSlot {

            public TokenWithTrailingTriviaSlot(int length, SyntaxKind kind, ImmutableArray<TriviaSlot> trailingTrivias)
                : base(length, kind) {
                TrailingTrivias = trailingTrivias;

                foreach (var t in trailingTrivias) {
                    AdjustLength(t);
                }
            }

            public override ImmutableArray<TriviaSlot> TrailingTrivias { get; }

        }

    }

}