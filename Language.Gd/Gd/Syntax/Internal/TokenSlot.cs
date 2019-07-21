using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    public partial class TokenSlot: Slot {

        TokenSlot(TextExtent textExtent, SyntaxKind kind)
            : base(textExtent, kind) {
        }

        public virtual ImmutableArray<TriviaSlot> LeadingTrivias  => ImmutableArray<TriviaSlot>.Empty;
        public virtual ImmutableArray<TriviaSlot> TrailingTrivias => ImmutableArray<TriviaSlot>.Empty;

        internal SyntaxToken Realize(SyntaxTree syntaxTree, SyntaxNode parent) {
            return new SyntaxToken(syntaxTree, parent, this);
        }

        public static TokenSlot Create(TextExtent textExtent, SyntaxKind kind,
                                       IReadOnlyList<TriviaSlot> leadingTrivias = null,
                                       IReadOnlyList<TriviaSlot> trailingTrivias = null) {

            if (leadingTrivias  != null && leadingTrivias.Any() &&
                trailingTrivias != null && trailingTrivias.Any()) {
                return new TokenWithTriviaSlot(
                    textExtent     : textExtent,
                    kind           : kind,
                    leadingTrivias : leadingTrivias.ToImmutableArray(),
                    trailingTrivias: trailingTrivias.ToImmutableArray());
            }

            if (leadingTrivias != null && leadingTrivias.Any()) {
                return new TokenWithLeadingTriviaSlot(
                    textExtent     : textExtent,
                    kind           : kind,
                    leadingTrivias : leadingTrivias.ToImmutableArray());
            }

            if (trailingTrivias != null && trailingTrivias.Any()) {
                return new TokenWithTrailingTriviaSlot(
                    textExtent     : textExtent,
                    kind           : kind,
                    trailingTrivias: trailingTrivias.ToImmutableArray());
            }

            return new TokenSlot(
                textExtent: textExtent,
                kind      : kind);
        }

    }

}