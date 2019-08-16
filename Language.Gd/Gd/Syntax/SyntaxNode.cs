#region Using Directives

using System;
using System.Diagnostics;
using System.Threading;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public abstract class SyntaxNode {

        private protected SyntaxNode(SyntaxTree syntaxTree, SyntaxSlot slot, SyntaxNode parent, int position) {

            SyntaxTree = syntaxTree ?? throw new ArgumentNullException(nameof(syntaxTree));
            Slot       = slot       ?? throw new ArgumentNullException(nameof(slot));
            Parent     = parent;
            Position   = position;

        }

        internal SyntaxSlot Slot { get; }

        public SyntaxTree SyntaxTree { get; }

        public bool IsMissing => Slot.IsMissing;

        [CanBeNull]
        public SyntaxNode Parent { get; }

        public SyntaxKind Kind => Slot.Kind;

        public int        ExtentStart => Position + Slot.GetLeadingTriviaWidth();
        public TextExtent Extent      => Slot.GetExtent(Position);
        public TextExtent FullExtent  => TextExtent.FromBounds(start: Position, end: EndPosition);

        internal int Position    { get; }
        internal int EndPosition => Position + Slot.FullLength;

        internal int Length     => Slot.Length;
        internal int FullLength => Slot.FullLength;

        internal bool IsList    => Slot.Kind == SyntaxKind.SyntaxList;
        internal int  SlotCount => Slot.SlotCount;

        // TODO Descendants Childs etc...

        /// <summary>
        /// Determines if the specified node is a descendant of this node.
        /// Returns true for current node.
        /// </summary>
        public bool Contains(SyntaxNode node) {
            if (node == null || !FullExtent.Contains(node.FullExtent)) {
                return false;
            }

            while (node != null) {
                if (node == this) {
                    return true;
                }

                node = node.Parent;
            }

            return false;
        }

        private protected SyntaxToken GetSyntaxToken(TokenSlot slot, int index) {

            return slot.Realize(SyntaxTree, this, GetChildPosition(index));

        }

        [CanBeNull]
        private protected SyntaxListNode GetSyntaxNode(ref SyntaxListNode field, [CanBeNull] SyntaxSlotList slot, int index) {

            if (slot == null) {
                return null;
            }

            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, (SyntaxListNode) slot.Realize(SyntaxTree, this, GetChildPosition(index)), null);
                result = field;
            }

            return result;
        }

        [CanBeNull]
        private protected TNode GetSyntaxNode<TNode>(ref TNode field, [CanBeNull] SyntaxSlot slot, int index) where TNode : SyntaxNode {

            if (slot == null) {
                return null;
            }

            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, (TNode) slot.Realize(SyntaxTree, this, GetChildPosition(index)), null);
                result = field;
            }

            return result;
        }

        private protected SyntaxNode GetSyntaxNodeElement(ref SyntaxNode element, int index) {
            Debug.Assert(IsList);

            var result = element;

            if (result == null) {
                var slot = (SyntaxSlot) Slot.GetSlot(index);
                // passing list's parent
                Interlocked.CompareExchange(ref element, slot.Realize(SyntaxTree, Parent, GetChildPosition(index)), null);
                result = element;
            }

            return result;
        }

        internal abstract SyntaxNode GetCachedSyntaxNode(int index);
        internal abstract SyntaxNode GetSyntaxNode(int index);

        internal virtual int GetChildPosition(int index) {
            int offset = 0;
            var slot   = Slot;
            while (index > 0) {
                index--;

                // Wenn es den Knoten bereits gibt, kann direkt dessen EndPosition
                // verwendet werden, und es muss nicht weiter bis zum ersten Slot
                // durchiteriert werden.
                var prevSibling = GetCachedSyntaxNode(index);
                if (prevSibling != null) {
                    return prevSibling.EndPosition + offset;
                }

                var slotChild = slot.GetSlot(index);
                if (slotChild != null) {
                    offset += slotChild.FullLength;
                }
            }

            return Position + offset;
        }

    }

}