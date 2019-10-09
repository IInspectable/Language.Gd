#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot: Slot {

        TokenSlot(string text, SyntaxKind kind)
            : base(text.Length, kind) {
            Text = text;

        }

        public string Text { get; }

        [CanBeNull]
        public virtual Slot LeadingTrivia => null;

        [CanBeNull]
        public virtual Slot TrailingTrivia => null;

        internal SyntaxToken Realize(SyntaxTree syntaxTree, SyntaxNode parent, int position) {
            return new SyntaxToken(syntaxTree, this, parent, position);
        }

        public static TokenSlot CreateMissingToken(SyntaxKind kind) {
            return new TokenSlot(String.Empty, kind: kind);
        }

        public static TokenSlot Create(string text, SyntaxKind kind,
                                       IReadOnlyList<TriviaSlot> leadingTrivia = null,
                                       IReadOnlyList<TriviaSlot> trailingTrivia = null) {

            text = text ?? string.Empty;
            if (kind == SyntaxKind.Eof) {
                text = String.Empty;
            }

            if (leadingTrivia  != null && leadingTrivia.Any() &&
                trailingTrivia != null && trailingTrivia.Any()) {

                return new TokenWithTriviaSlot(
                    text: text,
                    kind: kind,
                    leadingTrivia: SlotList.Create(leadingTrivia),
                    trailingTrivia: SlotList.Create(trailingTrivia));
            }

            if (leadingTrivia != null && leadingTrivia.Any()) {
                return new TokenWithLeadingTriviaSlot(
                    text: text,
                    kind: kind,
                    leadingTrivia: SlotList.Create(leadingTrivia));
            }

            if (trailingTrivia != null && trailingTrivia.Any()) {
                return new TokenWithTrailingTriviaSlot(
                    text: text,
                    kind: kind,
                    trailingTrivia: SlotList.Create(trailingTrivia));
            }

            return new TokenSlot(
                text: text,
                kind: kind);
        }

        public override int GetLeadingTriviaWidth() {
            return LeadingTrivia?.FullLength ?? 0;
        }

        public override int GetTrailingTriviaWidth() {
            return TrailingTrivia?.FullLength ?? 0;
        }

        public override void WriteTo(StringBuilder sb, bool includeLeadingTrivia, bool includeTrailingTrivia) {

            if (includeLeadingTrivia) {
                LeadingTrivia?.WriteTo(sb, includeLeadingTrivia: true, includeTrailingTrivia: true);
            }

            sb.Append(Text);

            if (includeTrailingTrivia) {
                TrailingTrivia?.WriteTo(sb, includeLeadingTrivia: true, includeTrailingTrivia: true);
            }
        }

    }

}