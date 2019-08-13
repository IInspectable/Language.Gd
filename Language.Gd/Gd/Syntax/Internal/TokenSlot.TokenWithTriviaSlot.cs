using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        class TokenWithTriviaSlot: TokenSlot {

            public TokenWithTriviaSlot(int fullLength, SyntaxKind kind,
                                       ImmutableArray<TriviaSlot> leadingTrivias, ImmutableArray<TriviaSlot> trailingTrivias)
                : base(fullLength, kind) {
                LeadingTrivias  = leadingTrivias;
                TrailingTrivias = trailingTrivias;
            }

            public override ImmutableArray<TriviaSlot> LeadingTrivias  { get; }
            public override ImmutableArray<TriviaSlot> TrailingTrivias { get; }

        }

    }

}