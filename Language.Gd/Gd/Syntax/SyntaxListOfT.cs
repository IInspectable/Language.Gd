using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd {

    // TODO SyntaxList
    public struct SyntaxList<TSyntax> where TSyntax : SyntaxNode {

        private readonly SyntaxListNode _node;

        // TODO 
        internal SyntaxList(SyntaxListNode slot) {
            _node = slot;
        }

        public TSyntax this[int index] => null;

    }

}