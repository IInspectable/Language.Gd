using System;
using System.Diagnostics;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    [Flags]
    enum SlotFlags {

        None                = 0,
        IsSkipedTokenTrivia = 1

    }

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(), nq}")]
    abstract class Slot {

        private int _fullLength;

        private string GetDebuggerDisplay() {
            return GetType().Name + " " + Kind;
        }

        protected Slot(int fullLength, SyntaxKind kind, SlotFlags slotFlags = SlotFlags.None) {
            _fullLength = fullLength;
            Kind        = kind;
            Flags       = slotFlags;
        }

        protected Slot(SyntaxKind kind, SlotFlags slotFlags = SlotFlags.None) {
            _fullLength = 0;
            Kind        = kind;
            Flags       = slotFlags;
        }

        public override string ToString() {
            return GetDebuggerDisplay();
        }

        public bool IsMissing           => FullLength == 0;
        public bool IsSkipedTokenTrivia => IsFlagPresent(SlotFlags.IsSkipedTokenTrivia);

        protected TSlot AdjustLength<TSlot>(TSlot slot) where TSlot : Slot {
            _fullLength += slot?.FullLength ?? 0;
            return slot;
        }

        public int FullLength => _fullLength;
        public int Length     => FullLength - GetLeadingTriviaWidth() - GetTrailingTriviaWidth();

        public virtual int GetLeadingTriviaWidth() {
            return FullLength != 0 ? GetFirstNonMissingChild().GetLeadingTriviaWidth() : 0;
        }

        public virtual int GetTrailingTriviaWidth() {
            return FullLength != 0 ? GetLastNonMissingChild().GetTrailingTriviaWidth() : 0;
        }

        internal Slot GetFirstNonMissingChild() {
            Slot node = this;

            do {
                Slot firstChild = null;
                for (int i = 0, n = node.SlotCount; i < n; i++) {
                    var child = node.GetSlot(i);
                    if (child != null && !child.IsMissing) {
                        firstChild = child;
                        break;
                    }
                }

                node = firstChild;
            } while (node?.SlotCount > 0);

            return node;
        }

        internal Slot GetLastNonMissingChild() {
            Slot node = this;

            do {
                Slot lastChild = null;
                for (int i = node.SlotCount - 1; i >= 0; i--) {
                    var child = node.GetSlot(i);
                    if (child != null && !child.IsMissing) {
                        lastChild = child;
                        break;
                    }
                }

                node = lastChild;
            } while (node?.SlotCount > 0);

            return node;
        }

        public SlotFlags Flags { get; }

        public SyntaxKind Kind { get; }

        public bool IsList => Kind == SyntaxKind.SyntaxList;

        public virtual int SlotCount => 0;

        public virtual Slot GetSlot(int index) => null;

        bool IsFlagPresent(SlotFlags flag) {
            return (Flags & flag) == flag;
        }

        public TextExtent GetExtent(int position) {
            // Start with the full span.
            var start  = position;
            var length = FullLength;

            // adjust for preceding trivia (avoid calling this twice, do not call Green.Width)
            var precedingWidth = GetLeadingTriviaWidth();
            start  += precedingWidth;
            length -= precedingWidth;

            // adjust for following trivia width
            length -= GetTrailingTriviaWidth();

            return new TextExtent(start, length);
        }

        public virtual int FindSlotIndexContainingOffset(int offset) {
            Debug.Assert(0 <= offset && offset < FullLength);

            int i;
            int accumulatedWidth = 0;
            for (i = 0;; i++) {
                Debug.Assert(i < SlotCount);
                var child = GetSlot(i);
                if (child != null) {
                    accumulatedWidth += child.FullLength;
                    if (offset < accumulatedWidth) {
                        break;
                    }
                }
            }

            return i;
        }

        public virtual int GetSlotOffset(int index) {
            int offset = 0;
            for (int i = 0; i < index; i++) {
                var child = GetSlot(i);
                if (child != null) {
                    offset += child.FullLength;
                }
            }

            return offset;
        }

    }

}