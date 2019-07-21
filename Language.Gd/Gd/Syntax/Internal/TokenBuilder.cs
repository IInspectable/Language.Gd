using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    class TokenBuilder {

        readonly List<TriviaSlot> _leadingTrivias  = new List<TriviaSlot>();
        readonly List<TriviaSlot> _trailingTrivias = new List<TriviaSlot>();
        readonly List<TriviaSlot> _pendingTrivias  = new List<TriviaSlot>();

        TextExtent? _tokenExtent;
        SyntaxKind? _tokenKind;

        private bool HasToken => _tokenExtent != null;

        public void AddTrivia(TriviaSlot trivia) {
            if (!HasToken) {
                _leadingTrivias.Add(trivia);
            } else {
                _trailingTrivias.Add(trivia);
            }
        }

        public void AddToken(TextExtent textExtent, SyntaxKind syntaxKind, out TokenSlot completedToken) {

            completedToken = null;

            if (HasToken) {

                if (_trailingTrivias.Any()) {

                    var index = _trailingTrivias.FindIndex(t => t.Kind == SyntaxKind.NewLineTrivia);
                    if (index >= 0) {
                        _pendingTrivias.AddRange(_trailingTrivias.GetRange(index: index + 1, count: _trailingTrivias.Count - index - 1));
                        _trailingTrivias.RemoveRange(index: index + 1, count: _trailingTrivias.Count - index - 1);
                    }
                }

                completedToken = TryCompleteToken();

                if (_pendingTrivias.Any()) {
                    _leadingTrivias.AddRange(_pendingTrivias);
                    _pendingTrivias.Clear();
                }

            }

            _tokenExtent = textExtent;
            _tokenKind   = syntaxKind;

        }

        [CanBeNull]
        public TokenSlot TryCompleteToken() {

            if (_tokenExtent == null || _tokenKind == null) {
                return null;
            }

            var tokenSlot = TokenSlot.Create(_tokenExtent.Value, _tokenKind.Value, _leadingTrivias, _trailingTrivias);

            Clear();

            return tokenSlot;
        }

        private void Clear() {

            _leadingTrivias.Clear();
            _tokenExtent = null;
            _tokenKind   = null;
            _trailingTrivias.Clear();
        }

    }

}