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

        public TextExtent FullExtent => TextExtent.FromBounds(start: Position, end: EndPosition);

        public TextExtent Extent {
            get {
                // Start with the full span.
                var start  = Position;
                var length = Slot.FullLength;

                // adjust for preceding trivia (avoid calling this twice, do not call Green.Width)
                var precedingWidth = Slot.GetLeadingTriviaWidth();
                start  += precedingWidth;
                length -= precedingWidth;

                // adjust for following trivia width
                length -= Slot.GetTrailingTriviaWidth();

                return new TextExtent(start, length);
            }
        }

        internal int  Position    { get; }
        internal int  EndPosition => Position + Slot.FullLength;
        internal bool IsList      => Slot.Kind == SyntaxKind.SyntaxList;
        internal int  SlotCount   => Slot.SlotCount;

        public int SpanStart => Position + Slot.GetLeadingTriviaWidth();

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

        /// <summary>
        /// The width of the node in characters, not including leading and trailing trivia.
        /// </summary>
        /// <remarks>
        /// The Width property returns the same value as Extent.Length, but is somewhat more efficient.
        /// </remarks>
        internal int Width => Slot.Length;

        /// <summary>
        /// The complete width of the node in characters, including leading and trailing trivia.
        /// </summary>
        /// <remarks>The FullWidth property returns the same value as Extent.Length, but is
        /// somewhat more efficient.</remarks>
        internal int FullWidth => Slot.FullLength;

        public SyntaxTree SyntaxTree { get; }

        public bool IsMissing => Slot.IsMissing;

        [CanBeNull]
        public SyntaxNode Parent { get; }

        public SyntaxKind Kind => Slot.Kind;

        internal SyntaxSlot Slot { get; }

        private protected SyntaxList<TNode> GetSyntaxNode<TNode, TSlot>(ref SyntaxListNode field, SyntaxSlotList<TSlot> slot, int index)
            where TNode : SyntaxNode
            where TSlot : SyntaxSlot {

            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, (SyntaxListNode) slot.Realize(SyntaxTree, this, GetChildPosition(index)), null);
                result = field;
            }

            return new SyntaxList<TNode>(result);
        }

        private protected SyntaxNode GetSyntaxNode(ref SyntaxNode field, SyntaxSlot slot, int index) {
            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, slot.Realize(SyntaxTree, this, GetChildPosition(index)), null);
                result = field;
            }

            return result;
        }

        private protected TNode GetSyntaxNode<TNode>(ref TNode field, SyntaxSlot slot, int index) where TNode : SyntaxNode {
            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, (TNode) slot.Realize(SyntaxTree, this, GetChildPosition(index)), null);
                result = field;
            }

            return result;
        }

        internal SyntaxNode GetSyntaxNodeElement(ref SyntaxNode element, int index) {
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

        public abstract SyntaxNode GetCachedSlot(int index);

        internal virtual int GetChildPosition(int index) {
            int offset = 0;
            var slot   = Slot;
            while (index > 0) {
                index--;

                // TODO GetChildPosition PerfOpt
                //var prevSibling = this.GetCachedSlot(index);
                //if (prevSibling != null)
                //{
                //    return prevSibling.EndPosition + offset;
                //}

                var slotChild = slot.GetSlot(index);
                if (slotChild != null) {
                    offset += slotChild.FullLength;
                }
            }

            return Position + offset;
        }

    }

}