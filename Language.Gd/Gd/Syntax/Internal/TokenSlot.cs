#region Using Directives

using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot: Slot {

        TokenSlot(int fullLength, SyntaxKind kind)
            : base(fullLength, kind) {
        }

        [CanBeNull]
        public virtual Slot LeadingTrivia => null;

        [CanBeNull]
        public virtual Slot TrailingTrivia => null;

        internal SyntaxToken Realize(SyntaxTree syntaxTree, SyntaxNode parent, int position) {
            return new SyntaxToken(syntaxTree, this, parent, position);
        }

        public static TokenSlot Create(int length, SyntaxKind kind,
                                       IReadOnlyList<TriviaSlot> leadingTrivia = null,
                                       IReadOnlyList<TriviaSlot> trailingTrivia = null) {

            if (leadingTrivia  != null && leadingTrivia.Any() &&
                trailingTrivia != null && trailingTrivia.Any()) {

                return new TokenWithTriviaSlot(
                    length: length,
                    kind: kind,
                    leadingTrivia: SlotList.Create(leadingTrivia),
                    trailingTrivia: SlotList.Create(trailingTrivia));
            }

            if (leadingTrivia != null && leadingTrivia.Any()) {
                return new TokenWithLeadingTriviaSlot(
                    length: length,
                    kind: kind,
                    leadingTrivia: SlotList.Create(leadingTrivia));
            }

            if (trailingTrivia != null && trailingTrivia.Any()) {
                return new TokenWithTrailingTriviaSlot(
                    length: length,
                    kind: kind,
                    trailingTrivia: SlotList.Create(trailingTrivia));
            }

            return new TokenSlot(
                fullLength: length,
                kind: kind);
        }

        public override int GetLeadingTriviaWidth() {
            return LeadingTrivia?.FullLength ?? 0;
        }

        public override int GetTrailingTriviaWidth() {
            return TrailingTrivia?.FullLength ?? 0;
        }

    }

}