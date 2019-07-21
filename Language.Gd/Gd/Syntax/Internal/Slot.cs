using System.Diagnostics;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(), nq}")]
    public abstract class Slot {

        private string GetDebuggerDisplay() {
            return Extent + " " + Kind;
        }

        protected Slot(TextExtent textExtent, SyntaxKind kind) {
            Start  = textExtent.Start;
            Length = textExtent.Length;
            Kind   = kind;
        }

        public override string ToString() {
            return GetDebuggerDisplay();
        }

        public bool IsMissing => Extent.IsMissing;

        public TextExtent Extent => new TextExtent(Start, Length);
        public int        Start  { get; }
        public int        End    => Start + Length;
        public int        Length { get; }
        public SyntaxKind Kind   { get; }

        public virtual int SlotCount => 0;

        public virtual Slot GetSlot(int index) => null;

    }

}