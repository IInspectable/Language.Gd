using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd {

    public partial class ChildNodesAndTokenList: IReadOnlyList<SyntaxNodeOrToken> {

        public ChildNodesAndTokenList(SyntaxNode node) {
            Node  = node;
            Count = CountNodes(node.Slot);
        }

        private static int CountNodes(SyntaxSlot nodeSlot) {

            int count = 0;

            for (int slotIndex = 0, slotCount = nodeSlot.SlotCount; slotIndex < slotCount; slotIndex++) {

                var childSlot = nodeSlot.GetSlot(slotIndex);
                if (childSlot == null) {
                    continue;
                }

                count += Occupancy(childSlot);
            }

            return count;
        }

        internal SyntaxNode Node { get; }

        public int Count { get; }

        public bool Any() {
            return Count != 0;
        }

        public SyntaxNodeOrToken First() {
            if (Any()) {
                return this[0];
            }

            throw new InvalidOperationException();
        }

        public Reversed Reverse() {
            return new Reversed(Node, Count);
        }

        public SyntaxNodeOrToken Last() {
            if (Any()) {
                return this[Count - 1];
            }

            throw new InvalidOperationException();
        }

        public Enumerator GetEnumerator() {
            if (Node == null) {
                return default;
            }

            return new Enumerator(Node, Count);
        }

        IEnumerator<SyntaxNodeOrToken> IEnumerable<SyntaxNodeOrToken>.GetEnumerator() {
            return new EnumaratorImpl(Node, Count);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new EnumaratorImpl(Node, Count);
        }

        public SyntaxNodeOrToken this[int index] {
            get {
                if (index < Count) {
                    return ItemInternal(Node, index);
                }

                throw new ArgumentOutOfRangeException();
            }
        }

        private static int Occupancy(Slot slot) {
            return slot.IsList ? slot.SlotCount : 1;
        }

        static SyntaxNodeOrToken ItemInternal(SyntaxNode node, int index) {

            var slotIndex = 0;
            var lstIndex  = index;
            var position  = node.Position;

            Slot childSlot;
            while (true) {

                childSlot = node.Slot.GetSlot(slotIndex);
                if (childSlot != null) {

                    int currentOccupancy = Occupancy(childSlot);
                    if (lstIndex < currentOccupancy) {
                        break;
                    }

                    lstIndex -= currentOccupancy;
                    position += childSlot.FullLength;
                }

                slotIndex++;
            }

            // Ein einzelnes Token
            if (childSlot is TokenSlot tokenSlot) {
                return new SyntaxNodeOrToken(node, tokenSlot, position);
            }

            var syntaxNode = node.GetSyntaxNode(slotIndex);
            if (syntaxNode == null) {
                // Kann das sein!?
                return default;
            }

            // Ein einzelner Knoten
            if (!syntaxNode.IsList) {
                return syntaxNode;
            }

            // Liste von Knoten => ToN
            var slotMember = syntaxNode.GetSyntaxNode(lstIndex);
            return slotMember;
        }

        internal static SyntaxNodeOrToken ChildThatContainsPosition(SyntaxNode node, int targetPosition) {
            Debug.Assert(node.FullExtent.Contains(targetPosition));
            Debug.Assert(!node.Slot.IsList);

            int slotIndex;
            var position = node.Position;

            Slot childSlot;
            for (slotIndex = 0;; slotIndex++) {

                childSlot = node.Slot.GetSlot(slotIndex);

                if (childSlot != null) {
                    var endPosition = position + childSlot.FullLength;
                    if (targetPosition < endPosition) {
                        break;
                    }

                    position = endPosition;
                }
            }

            // Ein einzelnes Token
            if (childSlot is TokenSlot tokenSlot) {
                return new SyntaxNodeOrToken(node, tokenSlot, position);
            }

            var syntaxNode = node.GetSyntaxNode(slotIndex);
            if (syntaxNode == null) {
                // Kann das sein!?
                return default;
            }

            // Ein einzelner Knoten
            if (!syntaxNode.IsList) {
                return syntaxNode;
            }

            // Liste von Knoten => ToN
            slotIndex = syntaxNode.Slot.FindSlotIndexContainingOffset(targetPosition - position);
            var slotMember = syntaxNode.GetSyntaxNode(slotIndex);

            return slotMember;
        }

        public struct Enumerator {

            private readonly SyntaxNode _node;
            private readonly int        _count;
            private          int        _index;

            internal Enumerator(SyntaxNode node, int count) {
                _node  = node;
                _count = count;
                _index = -1;

            }

            public bool MoveNext() {
                var newIndex = _index + 1;
                if (newIndex < _count) {
                    _index = newIndex;
                    return true;
                }

                return false;
            }

            public SyntaxNodeOrToken Current => ItemInternal(_node, _index);

            public void Reset() {
                _index = -1;
            }

        }

        class EnumaratorImpl: IEnumerator<SyntaxNodeOrToken> {

            private Enumerator _enumerator;

            public EnumaratorImpl(SyntaxNode node, int count) {
                _enumerator = new Enumerator(node, count);

            }

            public SyntaxNodeOrToken Current => _enumerator.Current;

            object IEnumerator.Current => _enumerator.Current;

            public bool MoveNext() {
                return _enumerator.MoveNext();
            }

            public void Reset() {
                _enumerator.Reset();
            }

            public void Dispose() {
            }

        }

    }

}