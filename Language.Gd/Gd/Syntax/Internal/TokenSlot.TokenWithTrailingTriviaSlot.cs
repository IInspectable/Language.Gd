namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        private class TokenWithTrailingTriviaSlot: TokenSlot {

            public TokenWithTrailingTriviaSlot(string text, SyntaxKind kind, Slot trailingTrivia)
                : base(text, kind) {

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