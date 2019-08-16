using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot: Slot {

        TokenSlot(int fullLength, SyntaxKind kind)
            : base(fullLength, kind) {
        }

        public virtual ImmutableArray<TriviaSlot> LeadingTrivias  => ImmutableArray<TriviaSlot>.Empty;
        public virtual ImmutableArray<TriviaSlot> TrailingTrivias => ImmutableArray<TriviaSlot>.Empty;

        internal SyntaxToken Realize(SyntaxTree syntaxTree, SyntaxNode parent) {
            return new SyntaxToken(syntaxTree, parent, this);
        }

        public static TokenSlot Create(int fullLength, SyntaxKind kind,
                                       IReadOnlyList<TriviaSlot> leadingTrivias = null,
                                       IReadOnlyList<TriviaSlot> trailingTrivias = null) {

            if (leadingTrivias  != null && leadingTrivias.Any() &&
                trailingTrivias != null && trailingTrivias.Any()) {
                return new TokenWithTriviaSlot(
                    fullLength: fullLength,
                    kind: kind,
                    leadingTrivias: leadingTrivias.ToImmutableArray(),
                    trailingTrivias: trailingTrivias.ToImmutableArray());
            }

            if (leadingTrivias != null && leadingTrivias.Any()) {
                return new TokenWithLeadingTriviaSlot(
                    fullLength: fullLength,
                    kind: kind,
                    leadingTrivias: leadingTrivias.ToImmutableArray());
            }

            if (trailingTrivias != null && trailingTrivias.Any()) {
                return new TokenWithTrailingTriviaSlot(
                    fullLength: fullLength,
                    kind: kind,
                    trailingTrivias: trailingTrivias.ToImmutableArray());
            }

            return new TokenSlot(
                fullLength: fullLength,
                kind: kind);
        }

        // TODO TokenSlot Trivias..
        public override int GetLeadingTriviaWidth() {
            return LeadingTrivias.Sum(l => l.FullLength);
        }

        public override int GetTrailingTriviaWidth() {
            return TrailingTrivias.Sum(l => l.FullLength);
        }

    }

}