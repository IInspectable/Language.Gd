using System.Collections.Immutable;

namespace Pharmatechnik.Language.Gd.Internal {

    abstract class SyntaxSlotList: SyntaxSlot {

        protected SyntaxSlotList(SyntaxKind kind): base(kind) {
        }

    }

    class SyntaxSlotList<T>: SyntaxSlotList where T : SyntaxSlot {

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

        public override SyntaxNode Realize(SyntaxTree syntaxTree, SyntaxNode parent, int position) {
            return new SyntaxListNode(syntaxTree, this, parent, position);
        }
        
    }

}