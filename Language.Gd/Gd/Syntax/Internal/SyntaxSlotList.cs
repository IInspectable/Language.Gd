using Pharmatechnik.Language.Text;

using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    class SyntaxSlotList<T>: Slot where T : SyntaxSlot {

        private readonly ImmutableArray<T> _slots;

        internal SyntaxSlotList(TextExtent textExtent, ImmutableArray<T> slots)
            : base(textExtent, SyntaxKind.SyntaxList) {
            _slots = slots;
        }

        public override int SlotCount => _slots.Length;

        public override Slot GetSlot(int index) {
            return _slots[index];
        }

    }

}