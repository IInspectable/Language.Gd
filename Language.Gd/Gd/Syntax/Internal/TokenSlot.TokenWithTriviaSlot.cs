using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        class TokenWithTriviaSlot: TokenSlot {

            public TokenWithTriviaSlot(int length, SyntaxKind kind,
                                       ImmutableArray<TriviaSlot> leadingTrivia, ImmutableArray<TriviaSlot> trailingTrivia)
                : base(length, kind) {

                LeadingTrivia  = leadingTrivia;
                TrailingTrivia = trailingTrivia;

                foreach (var t in leadingTrivia) {
                    AdjustLength(t);
                }

                foreach (var t in trailingTrivia) {
                    AdjustLength(t);
                }
            }

            public override ImmutableArray<TriviaSlot> LeadingTrivia  { get; }
            public override ImmutableArray<TriviaSlot> TrailingTrivia { get; }

        }

    }

}