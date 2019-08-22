using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot: Slot {

        TokenSlot(int fullLength, SyntaxKind kind)
            : base(fullLength, kind) {
        }

        public virtual ImmutableArray<TriviaSlot> LeadingTrivia  => ImmutableArray<TriviaSlot>.Empty;
        public virtual ImmutableArray<TriviaSlot> TrailingTrivia => ImmutableArray<TriviaSlot>.Empty;

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
                    leadingTrivia: leadingTrivia.ToImmutableArray(),
                    trailingTrivia: trailingTrivia.ToImmutableArray());
            }

            if (leadingTrivia != null && leadingTrivia.Any()) {
                return new TokenWithLeadingTriviaSlot(
                    length: length,
                    kind: kind,
                    leadingTrivia: leadingTrivia.ToImmutableArray());
            }

            if (trailingTrivia != null && trailingTrivia.Any()) {
                return new TokenWithTrailingTriviaSlot(
                    length: length,
                    kind: kind,
                    trailingTrivia: trailingTrivia.ToImmutableArray());
            }

            return new TokenSlot(
                fullLength: length,
                kind: kind);
        }

        // TODO TokenSlot Trivias..
        public override int GetLeadingTriviaWidth() {
            return LeadingTrivia.Sum(l => l.FullLength);
        }

        public override int GetTrailingTriviaWidth() {
            return TrailingTrivia.Sum(l => l.FullLength);
        }

    }

}