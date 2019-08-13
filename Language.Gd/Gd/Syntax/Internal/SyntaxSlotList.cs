using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    class SyntaxSlotList<T>: Slot where T : SyntaxSlot {

        private readonly ImmutableArray<T> _slots;

        internal SyntaxSlotList(ImmutableArray<T> slots)
            : base(SyntaxKind.SyntaxList) {
            _slots = slots;
            foreach (var slot in slots) {
                AdjustLength(slot);
            }
        }

        public override int SlotCount => _slots.Length;

        public override Slot GetSlot(int index) {
            return _slots[index];
        }

    }

}