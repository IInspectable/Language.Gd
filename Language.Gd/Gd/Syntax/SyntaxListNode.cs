using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd {

    class SyntaxListNode: SyntaxNode  {

        internal SyntaxListNode(SyntaxTree syntaxTree, SyntaxSlotList slotList, SyntaxNode parent, int position): base(syntaxTree, slotList, parent, position) {
        }

        // TODO GetCachedSlot
        public override SyntaxNode GetCachedSyntaxNode(int index) {
            throw new System.NotImplementedException();
        }

    }

}