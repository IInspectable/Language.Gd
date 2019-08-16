using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        private class TokenWithLeadingTriviaSlot: TokenSlot {

            public TokenWithLeadingTriviaSlot(int length, SyntaxKind kind, ImmutableArray<TriviaSlot> leadingTrivias)
                : base(length, kind) {
                LeadingTrivias = leadingTrivias;

                foreach (var t in leadingTrivias) {
                    AdjustLength(t);
                }

            }

            public override ImmutableArray<TriviaSlot> LeadingTrivias { get; }

        }

    }

}