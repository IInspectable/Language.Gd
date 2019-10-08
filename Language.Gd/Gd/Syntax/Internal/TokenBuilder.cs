using System.Collections.Generic;
using System.Linq;

namespace Pharmatechnik.Language.Gd.Internal {

    class TokenBuilder {

        readonly List<TriviaSlot> _leadingTrivia  = new List<TriviaSlot>();
        readonly List<TriviaSlot> _trailingTrivia = new List<TriviaSlot>();
        readonly List<TriviaSlot> _pendingTrivia  = new List<TriviaSlot>();

        string      _tokenText;
        int?        _tokenStart;
        SyntaxKind? _tokenKind;

        private bool HasToken => _tokenStart != null;

        public void AddTrivia(TriviaSlot trivia) {
            if (!HasToken) {
                _leadingTrivia.Add(trivia);
            } else {
                _trailingTrivia.Add(trivia);
            }
        }

        public void AddToken(int start, string text, SyntaxKind syntaxKind, out (int Start, TokenSlot Token) completedTokenInfo) {

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

            _tokenStart = start;
            _tokenText  = text;
            _tokenKind  = syntaxKind;

        }

        public (int Start, TokenSlot Token) TryCompleteToken() {

            if (_tokenStart == null || _tokenKind == null) {
                return (Start: 0, Token: null);
            }

            var start     = _tokenStart.Value;
            var tokenKind = _tokenKind.Value;
            var tokenSlot = TokenSlot.Create(_tokenText, tokenKind, _leadingTrivia, _trailingTrivia);

            Clear();

            return (Start: start, Token: tokenSlot);
        }

        private void Clear() {

            _leadingTrivia.Clear();
            _tokenText  = null;
            _tokenStart = null;
            _tokenKind  = null;
            _trailingTrivia.Clear();
        }

    }

}