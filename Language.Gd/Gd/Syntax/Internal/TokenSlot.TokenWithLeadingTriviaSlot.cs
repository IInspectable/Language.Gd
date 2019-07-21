using System.Collections.Immutable;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    public partial class TokenSlot {

        private class TokenWithLeadingTriviaSlot: TokenSlot {

            public TokenWithLeadingTriviaSlot(TextExtent textExtent, SyntaxKind kind, ImmutableArray<TriviaSlot> leadingTrivias)
                : base(textExtent, kind) {
                LeadingTrivias = leadingTrivias;
            }

            public override ImmutableArray<TriviaSlot> LeadingTrivias { get; }

        }

    }

}