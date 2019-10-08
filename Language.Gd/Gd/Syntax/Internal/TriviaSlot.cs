﻿namespace Pharmatechnik.Language.Gd.Internal {

    class TriviaSlot: Slot {

        TriviaSlot(string text, SyntaxKind kind, SlotFlags flags)
            : base(text.Length, kind, flags) {

        }

        internal static TriviaSlot Create(string text, SyntaxKind kind, bool isSkipedTokenTrivia) {
            var flags = isSkipedTokenTrivia ? SlotFlags.IsSkipedTokenTrivia : SlotFlags.None;
            return new TriviaSlot(text, kind, flags);
        }

        internal SyntaxTrivia Realize(SyntaxTree syntaxTree, SyntaxToken token, int position) {
            return new SyntaxTrivia(syntaxTree: syntaxTree, token: token, slot: this, position: position);
        }

        public override int GetLeadingTriviaWidth() {
            return 0;
        }

        public override int GetTrailingTriviaWidth() {
            return 0;
        }

    }

}