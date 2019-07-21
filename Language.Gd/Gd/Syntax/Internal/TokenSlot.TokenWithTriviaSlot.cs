using System.Collections.Immutable;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    partial class TokenSlot {

        class TokenWithTriviaSlot: TokenSlot {

            public TokenWithTriviaSlot(TextExtent textExtent, SyntaxKind kind,
                                       ImmutableArray<TriviaSlot> leadingTrivias, ImmutableArray<TriviaSlot> trailingTrivias)
                : base(textExtent, kind) {
                LeadingTrivias  = leadingTrivias;
                TrailingTrivias = trailingTrivias;
            }

            public override ImmutableArray<TriviaSlot> LeadingTrivias  { get; }
            public override ImmutableArray<TriviaSlot> TrailingTrivias { get; }

        }

    }

}