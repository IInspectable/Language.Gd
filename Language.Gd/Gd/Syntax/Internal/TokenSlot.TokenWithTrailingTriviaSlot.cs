using System.Collections.Immutable;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    public partial class TokenSlot {

        private class TokenWithTrailingTriviaSlot: TokenSlot {

            public TokenWithTrailingTriviaSlot(TextExtent textExtent, SyntaxKind kind, ImmutableArray<TriviaSlot> trailingTrivias)
                : base(textExtent, kind) {
                TrailingTrivias = trailingTrivias;
            }

            public override ImmutableArray<TriviaSlot> TrailingTrivias { get; }

        }

    }

}