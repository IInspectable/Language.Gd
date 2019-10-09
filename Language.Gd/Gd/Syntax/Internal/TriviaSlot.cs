using System.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    class TriviaSlot: Slot {

        TriviaSlot(string text, SyntaxKind kind, SlotFlags flags)
            : base(text.Length, kind, flags) {
            Text = text;

        }

        public string Text { get; }

        internal static TriviaSlot Create(string text, SyntaxKind kind, bool isSkipedTokenTrivia) {
            var flags = isSkipedTokenTrivia ? SlotFlags.IsSkipedTokenTrivia : SlotFlags.None;
            return new TriviaSlot(text, kind, flags);
        }

        internal SyntaxTrivia Realize( SyntaxToken token, int position) {
            return new SyntaxTrivia(token: token, slot: this, position: position);
        }

        public override int GetLeadingTriviaWidth() {
            return 0;
        }

        public override int GetTrailingTriviaWidth() {
            return 0;
        }

        public override void WriteTo(StringBuilder sb, bool includeLeadingTrivia, bool includeTrailingTrivia) {
            sb.Append(Text);
        }

    }

}