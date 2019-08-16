using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        class TokenWithTriviaSlot: TokenSlot {

            public TokenWithTriviaSlot(int length, SyntaxKind kind,
                                       ImmutableArray<TriviaSlot> leadingTrivias, ImmutableArray<TriviaSlot> trailingTrivias)
                : base(length, kind) {
                LeadingTrivias  = leadingTrivias;
                TrailingTrivias = trailingTrivias;

                foreach (var t in leadingTrivias) {
                    AdjustLength(t);
                }

                foreach (var t in trailingTrivias) {
                    AdjustLength(t);
                }
            }

            public override ImmutableArray<TriviaSlot> LeadingTrivias  { get; }
            public override ImmutableArray<TriviaSlot> TrailingTrivias { get; }

        }

    }

}