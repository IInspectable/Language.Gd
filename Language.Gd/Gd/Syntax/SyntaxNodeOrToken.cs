#region Using Directives

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public struct SyntaxNodeOrToken {

        [CanBeNull]
        readonly TokenSlot _tokenSlot;

        [CanBeNull]
        readonly SyntaxNode _nodeOrParent;

        private readonly int _position;

        internal SyntaxNodeOrToken([NotNull] SyntaxNode syntaxNode) {
            _nodeOrParent = syntaxNode;
            _position     = syntaxNode.Position;
            _tokenSlot    = null;
        }

        internal SyntaxNodeOrToken([NotNull] SyntaxNode tokenParent, TokenSlot tokenSlot, int tokenPosition) {
            _nodeOrParent = tokenParent;
            _position     = tokenPosition;
            _tokenSlot    = tokenSlot;
        }

        public bool IsNode    => _tokenSlot == null;
        public bool IsToken   => _tokenSlot != null;
        public bool IsMissing => _tokenSlot?.IsMissing ?? _nodeOrParent?.IsMissing ?? false;

        public SyntaxKind Kind       => _tokenSlot?.Kind ?? _nodeOrParent?.Kind ?? SyntaxKind.None;
        public SyntaxNode Parent     => _tokenSlot != null ? _nodeOrParent : _nodeOrParent?.Parent;
        public SyntaxTree SyntaxTree => _nodeOrParent?.SyntaxTree;

        public int ExtentStart {
            get {
                if (_tokenSlot != null) {
                    return _position + _tokenSlot.GetLeadingTriviaWidth();
                }

                if (_nodeOrParent != null) {
                    return _nodeOrParent.ExtentStart;
                }

                return default;
            }
        }

        public TextExtent Extent {
            get {

                if (_tokenSlot != null) {
                    return AsToken().Extent;
                }

                if (_nodeOrParent != null) {
                    return _nodeOrParent.Extent;
                }

                return default;
            }
        }

        internal int FullLength  => _tokenSlot?.FullLength ?? _nodeOrParent?.FullLength ?? 0;
        public   int Position    => _position;
        internal int EndPosition => _position + FullLength;

        public TextExtent FullExtent {
            get {

                if (_tokenSlot != null) {
                    return AsToken().FullExtent;
                }

                if (_nodeOrParent != null) {
                    return _nodeOrParent.FullExtent;
                }

                return default;
            }
        }

        public SyntaxNode AsNode() {

            if (IsToken) {
                return null;
            }

            return _nodeOrParent;
        }

        public SyntaxToken AsToken() {

            if (_tokenSlot == null || _nodeOrParent == null) {
                return default;
            }

            return new SyntaxToken(_nodeOrParent.SyntaxTree, _tokenSlot, _nodeOrParent, _position);
        }

        public ChildNodesAndTokenList ChildNodesAndTokens() {

            if (IsToken || _nodeOrParent == null) {
                return default;
            }

            return _nodeOrParent.ChildNodesAndTokens();
        }

        public override string ToString() {

            if (_tokenSlot != null) {
                return _tokenSlot.ToString();
            }

            if (_nodeOrParent != null) {
                return _nodeOrParent.ToString();
            }

            return string.Empty;
        }

        public static implicit operator SyntaxNodeOrToken(SyntaxNode node) {
            return new SyntaxNodeOrToken(node);
        }

        public static implicit operator SyntaxNodeOrToken(SyntaxToken token) {
            return new SyntaxNodeOrToken(token.Parent, token.Slot, token.Position);
        }

    }

}