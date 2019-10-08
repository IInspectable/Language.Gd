namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        private class TokenWithLeadingTriviaSlot: TokenSlot {

            public TokenWithLeadingTriviaSlot(string text, SyntaxKind kind, Slot leadingTrivia)
                : base(text, kind) {

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