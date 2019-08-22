using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        private class TokenWithLeadingTriviaSlot: TokenSlot {

            public TokenWithLeadingTriviaSlot(int length, SyntaxKind kind, ImmutableArray<TriviaSlot> leadingTrivia)
                : base(length, kind) {

                LeadingTrivia = leadingTrivia;

                foreach (var t in leadingTrivia) {
                    AdjustLength(t);
                }

            }

            public override ImmutableArray<TriviaSlot> LeadingTrivia { get; }

        }

    }

}