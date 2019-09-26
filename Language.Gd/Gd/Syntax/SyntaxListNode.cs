#region Using Directives

using System;

using Pharmatechnik.Language.Gd.Internal;

#endregion

namespace Pharmatechnik.Language.Gd {

    // TODO Perf Opt für 1,2,3 Elemente => GD-Analyse
    class SyntaxListNode: SyntaxNode {

        readonly SyntaxNode[] _children;

        internal SyntaxListNode(SyntaxTree syntaxTree, SlotList slotList, SyntaxNode parent, int position)
            : base(syntaxTree: syntaxTree, slot: slotList, parent: parent, position: position) {
            _children = new SyntaxNode[slotList.SlotCount];
        }

        private protected override SyntaxNode GetCachedSyntaxNode(int index) {
            return _children[index];
        }

        internal override SyntaxNode GetSyntaxNode(int index) {
            return GetSyntaxNodeElement(ref _children[index], index: index);

        }

        public override void Accept(SyntaxVisitor visitor) {
            throw new InvalidOperationException();
        }

        public override TResult Accept<TResult>(SyntaxVisitor<TResult> visitor) {
            throw new InvalidOperationException();
        }

    }

}