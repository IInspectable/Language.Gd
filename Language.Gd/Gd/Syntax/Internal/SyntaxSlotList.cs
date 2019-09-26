using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

using JetBrains.Annotations;

namespace Pharmatechnik.Language.Gd.Internal {

    abstract class SlotList: Slot {

        private readonly ImmutableArray<Slot> _slots;

        internal SlotList(ImmutableArray<Slot> slots)
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

    static class SyntaxSlotList {

        // TODO Spezialisierungen 1,2,3 Slots??

        public static SyntaxSlotList<TSyntaxSlot> Create<TSyntaxSlot>(IEnumerable<SyntaxSlot> slots) where TSyntaxSlot : SyntaxSlot {
            return new SyntaxSlotList<TSyntaxSlot>(
                slots.Where(slot => slot != null)
                     .Cast<TSyntaxSlot>() // Sicherstellen, dass alle elemente vom Typ TSlot sind
                     .Cast<Slot>()
                     .ToImmutableArray());
        }

    }

    // Dient nur, um den Code lesbarer zu gestalten
    class SyntaxSlotList<T>: SlotList where T : SyntaxSlot {

        internal SyntaxSlotList(ImmutableArray<Slot> slots)
            : base(slots) {

        }

        [UsedImplicitly]
        public Type SlotType => typeof(T);

    }

}