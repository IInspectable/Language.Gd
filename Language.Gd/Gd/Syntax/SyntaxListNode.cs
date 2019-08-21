using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd {

    // TODO Perf Opt für 1,2,3 Elemente => GD-Analyse
    class SyntaxListNode: SyntaxNode {

        readonly SyntaxNode[] _children;

        internal SyntaxListNode(SyntaxTree syntaxTree, SyntaxSlotList slotList, SyntaxNode parent, int position): base(syntaxTree, slotList, parent, position) {
            _children = new SyntaxNode[slotList.SlotCount];
        }

        internal override SyntaxNode GetCachedSyntaxNode(int index) {
            return _children[index];
        }

        internal override SyntaxNode GetSyntaxNode(int index) {
            return GetSyntaxNodeElement(ref _children[index], index: index);

        }

    }

}