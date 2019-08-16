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

        internal SyntaxToken Realize(SyntaxTree syntaxTree, SyntaxNode parent, int position) {
            return new SyntaxToken(syntaxTree, this, parent, position);
        }

        public static TokenSlot Create(int length, SyntaxKind kind,
                                       IReadOnlyList<TriviaSlot> leadingTrivias = null,
                                       IReadOnlyList<TriviaSlot> trailingTrivias = null) {

            if (leadingTrivias  != null && leadingTrivias.Any() &&
                trailingTrivias != null && trailingTrivias.Any()) {

                return new TokenWithTriviaSlot(
                    length: length,
                    kind: kind,
                    leadingTrivias: leadingTrivias.ToImmutableArray(),
                    trailingTrivias: trailingTrivias.ToImmutableArray());
            }

            if (leadingTrivias != null && leadingTrivias.Any()) {
                return new TokenWithLeadingTriviaSlot(
                    length: length,
                    kind: kind,
                    leadingTrivias: leadingTrivias.ToImmutableArray());
            }

            if (trailingTrivias != null && trailingTrivias.Any()) {
                return new TokenWithTrailingTriviaSlot(
                    length: length,
                    kind: kind,
                    trailingTrivias: trailingTrivias.ToImmutableArray());
            }

            return new TokenSlot(
                fullLength: length,
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