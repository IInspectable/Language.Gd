using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd {

    public struct SyntaxList<T> where T : SyntaxNode {

        private readonly SyntaxNode                 _parent;
        private readonly SyntaxListSlot<SyntaxSlot> _slot;

        // TODO 
        internal SyntaxList(SyntaxNode parent, SyntaxListSlot<SyntaxSlot> slot) {
            _parent = parent;
            _slot   = slot;
        }

    }

}