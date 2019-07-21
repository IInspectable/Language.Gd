using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal
{
    public abstract class SyntaxListSlot: Slot {

        protected SyntaxListSlot(TextExtent textExtent)
            : base(textExtent, SyntaxKind.SyntaxList) {
        }

    }

}