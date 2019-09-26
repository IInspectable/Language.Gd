#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

#endregion

namespace Pharmatechnik.Language.Gd.Internal {

    // TODO Spezialisierungen 1,2,3 Slots?? Siehe auch SyntaxNodeList
    class SlotList: Slot {

        private readonly ImmutableArray<Slot> _slots;

        private SlotList(ImmutableArray<Slot> slots)
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

        public static SlotList Create(IReadOnlyList<TriviaSlot> slots) {

            return new SlotList(slots.Cast<Slot>().ToImmutableArray());
        }

        public static SlotList Create<TSyntaxSlot>(IEnumerable<SyntaxSlot> slots) where TSyntaxSlot : SyntaxSlot {
            return new SlotList(
                slots.Where(slot => slot != null)
                     .Cast<TSyntaxSlot>() // Sicherstellen, dass alle elemente vom Typ TSlot sind
                     .Cast<Slot>()
                     .ToImmutableArray());
        }

        public SyntaxNodeList Realize(SyntaxTree syntaxTree, SyntaxNode parent, int position) {
            return new SyntaxNodeList(syntaxTree, this, parent, position);
        }

    }

}