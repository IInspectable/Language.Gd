namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        private class TokenWithLeadingTriviaSlot: TokenSlot {

            public TokenWithLeadingTriviaSlot(int length, SyntaxKind kind, Slot leadingTrivia)
                : base(length, kind) {

                LeadingTrivia = leadingTrivia;

                AdjustLength(leadingTrivia);

            }

            public override Slot LeadingTrivia { get; }

            public override int GetTrailingTriviaWidth() {
                return 0;
            }

        }

    }

}