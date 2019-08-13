using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        private class TokenWithLeadingTriviaSlot: TokenSlot {

            public TokenWithLeadingTriviaSlot(int fullLength, SyntaxKind kind, ImmutableArray<TriviaSlot> leadingTrivias)
                : base(fullLength, kind) {
                LeadingTrivias = leadingTrivias;
            }

            public override ImmutableArray<TriviaSlot> LeadingTrivias { get; }

        }

    }

}