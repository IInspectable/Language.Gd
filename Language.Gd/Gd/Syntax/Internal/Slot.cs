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

        private string GetDebuggerDisplay() {
            return Extent + " " + Kind;
        }

        protected Slot(TextExtent textExtent, SyntaxKind kind, SlotFlags slotFlags = SlotFlags.None) {
            Start  = textExtent.Start;
            Length = textExtent.Length;
            Kind   = kind;
            Flags  = slotFlags;
        }

        public override string ToString() {
            return GetDebuggerDisplay();
        }

        public bool IsMissing           => Extent.IsMissing;
        public bool IsSkipedTokenTrivia => IsFlagPresent(SlotFlags.IsSkipedTokenTrivia);

        public SlotFlags  Flags  { get; }
        public TextExtent Extent => new TextExtent(Start, Length);
        public int        Start  { get; }
        public int        End    => Start + Length;
        public int        Length { get; }
        public SyntaxKind Kind   { get; }

        public virtual int SlotCount => 0;

        public virtual Slot GetSlot(int index) => null;

        bool IsFlagPresent(SlotFlags flag) {
            return (Flags & flag) == flag;
        }

    }

}