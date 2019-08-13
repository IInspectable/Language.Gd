using System;
using System.Linq;
using System.Threading;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd {

    public abstract class SyntaxNode {

        private protected SyntaxNode(SyntaxTree syntaxTree, SyntaxSlot slot, SyntaxNode parent, int position) {
            SyntaxTree = syntaxTree ?? throw new ArgumentNullException(nameof(syntaxTree));
            Slot       = slot       ?? throw new ArgumentNullException(nameof(slot));
            Parent     = parent;
            Position   = position;

        }

        public TextExtent FullSpan => TextExtent.FromBounds(start: Position, end: EndPosition);

        public TextExtent Span {
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

        // TODO 
        internal int  Position    { get; }
        internal int  EndPosition => Position + Slot.FullLength;
        internal bool IsList      => Slot.Kind == SyntaxKind.SyntaxList;

        internal int SlotCount => Slot.SlotCount;

        /// <summary>
        /// Same as accessing <see cref="TextSpan.Start"/> on <see cref="Span"/>.
        /// </summary>
        /// <remarks>
        /// Slight performance improvement.
        /// </remarks>
        //public int SpanStart => Position + Green.GetLeadingTriviaWidth();

        ///// <summary>
        ///// The width of the node in characters, not including leading and trailing trivia.
        ///// </summary>
        ///// <remarks>
        ///// The Width property returns the same value as Span.Length, but is somewhat more efficient.
        ///// </remarks>
        //internal int Width => this.Green.Width;

        ///// <summary>
        ///// The complete width of the node in characters, including leading and trailing trivia.
        ///// </summary>
        ///// <remarks>The FullWidth property returns the same value as FullSpan.Length, but is
        ///// somewhat more efficient.</remarks>
        //internal int FullWidth => this.Green.FullWidth;

        public SyntaxTree SyntaxTree { get; }

        [CanBeNull]
        public SyntaxNode Parent { get; }

        // public TextExtent Extent => Slot.Extent;
        public SyntaxKind Kind => Slot.Kind;

        internal SyntaxSlot Slot { get; }

        private protected SyntaxList<T> GetSyntaxNode<T, TSlot>(ref SyntaxList<T> field, SyntaxSlotList<TSlot> slots, int index)
            where T : SyntaxNode
            where TSlot : SyntaxSlot {
            // TODO Implement GetSyntaxNode for Lists
            return default;
        }

        private protected SyntaxNode GetSyntaxNode(ref SyntaxNode field, SyntaxSlot slot, int index) {
            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, slot.Realize(SyntaxTree, this, GetChildPosition(index)), null);
                result = field;
            }

            return result;
        }

        private protected T GetSyntaxNode<T>(ref T field, SyntaxSlot slot, int index) where T : SyntaxNode {
            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, (T) slot.Realize(SyntaxTree, this, GetChildPosition(index)), null);
                result = field;
            }

            return result;
        }

        //public abstract SyntaxNode GetCachedSlot(int index);

        // TODO GetChildPosition

        internal virtual int GetChildPosition(int index) {
            int offset = 0;
            var green  = Slot;
            while (index > 0) {
                index--;
                //var prevSibling = this.GetCachedSlot(index);
                //if (prevSibling != null)
                //{
                //    return prevSibling.EndPosition + offset;
                //}

                var greenChild = green.GetSlot(index);
                if (greenChild != null) {
                    offset += greenChild.FullLength;
                }
            }

            return Position + offset;
        }

    }

}