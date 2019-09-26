namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        class TokenWithTriviaSlot: TokenSlot {

            public TokenWithTriviaSlot(int length, SyntaxKind kind,
                                       Slot leadingTrivia, Slot trailingTrivia)
                : base(length, kind) {

                LeadingTrivia  = leadingTrivia;
                TrailingTrivia = trailingTrivia;

                AdjustLength(leadingTrivia);
                AdjustLength(trailingTrivia);
            }

            public override Slot LeadingTrivia  { get; }
            public override Slot TrailingTrivia { get; }

        }

    }

}