using System.Collections.Generic;
using System.Linq;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    class TokenBuilder {

        readonly List<TriviaSlot> _leadingTrivia  = new List<TriviaSlot>();
        readonly List<TriviaSlot> _trailingTrivia = new List<TriviaSlot>();
        readonly List<TriviaSlot> _pendingTrivia  = new List<TriviaSlot>();

        TextExtent? _tokenExtent;
        SyntaxKind? _tokenKind;

        private bool HasToken => _tokenExtent != null;

        public void AddTrivia(TriviaSlot trivia) {
            if (!HasToken) {
                _leadingTrivia.Add(trivia);
            } else {
                _trailingTrivia.Add(trivia);
            }
        }

        public void AddToken(TextExtent textExtent, SyntaxKind syntaxKind, out (int Start, TokenSlot Token) completedTokenInfo) {

            completedTokenInfo = (-1, null);

            if (HasToken) {

                if (_trailingTrivia.Any()) {

                    var index = _trailingTrivia.FindIndex(t => t.Kind == SyntaxKind.NewLineTrivia);
                    if (index >= 0) {
                        _pendingTrivia.AddRange(_trailingTrivia.GetRange(index: index + 1, count: _trailingTrivia.Count - index - 1));
                        _trailingTrivia.RemoveRange(index: index + 1, count: _trailingTrivia.Count - index - 1);
                    }
                }

                completedTokenInfo = TryCompleteToken();

                if (_pendingTrivia.Any()) {
                    _leadingTrivia.AddRange(_pendingTrivia);
                    _pendingTrivia.Clear();
                }

            }

            _tokenExtent = textExtent;
            _tokenKind   = syntaxKind;

        }

        public (int Start, TokenSlot Token) TryCompleteToken() {

            if (_tokenExtent == null || _tokenKind == null) {
                return (Start: 0, Token: null);
            }

            var start     = _tokenExtent.Value.Start;
            var tokenSlot = TokenSlot.Create(_tokenExtent.Value.Length, _tokenKind.Value, _leadingTrivia, _trailingTrivia);

            Clear();

            return (Start: start, Token: tokenSlot);
        }

        private void Clear() {

            _leadingTrivia.Clear();
            _tokenExtent = null;
            _tokenKind   = null;
            _trailingTrivia.Clear();
        }

    }

}