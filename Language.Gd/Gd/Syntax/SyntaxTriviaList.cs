using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd {

    public struct SyntaxTriviaList: IReadOnlyList<SyntaxTrivia> {

        private readonly ImmutableArray<TriviaSlot> _trivias;

        internal SyntaxTriviaList(SyntaxToken token, ImmutableArray<TriviaSlot> trivias, int position) {
            _trivias = trivias;
            Token    = token;
            Position = position;

        }

        public SyntaxToken Token    { get; }
        public int         Position { get; }

        public int Count => _trivias.Length;

        // TODO Evtl Caching?
        public SyntaxTrivia this[int index] {
            get { return _trivias[index].Realize(Token.SyntaxTree, Token, GetChildPosition(index)); }
        }

        int GetChildPosition(int index) {
            int offset = 0;
            while (index > 0) {
                index--;
                var triviaSlot = _trivias[index];
                if (triviaSlot != null) {
                    offset += triviaSlot.FullLength;
                }
            }

            return Position + offset;
        }

        // TODO Enumerator??
        public IEnumerator<SyntaxTrivia> GetEnumerator() {
            for (int i = 0; i < _trivias.Length; i++) {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

    }

}