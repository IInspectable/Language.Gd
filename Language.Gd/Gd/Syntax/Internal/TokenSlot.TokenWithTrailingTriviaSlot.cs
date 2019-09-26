namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        private class TokenWithTrailingTriviaSlot: TokenSlot {

            public TokenWithTrailingTriviaSlot(int length, SyntaxKind kind, Slot trailingTrivia)
                : base(length, kind) {

                TrailingTrivia = trailingTrivia;
                AdjustLength(trailingTrivia);
            }

            public override Slot TrailingTrivia { get; }

            public override int GetLeadingTriviaWidth() {
                return 0;
            }

        }

    }

}