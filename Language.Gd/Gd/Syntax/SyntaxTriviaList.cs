using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;

using Pharmatechnik.Language.Gd.Internal;

namespace Pharmatechnik.Language.Gd {

    public struct SyntaxTriviaList: IReadOnlyList<SyntaxTrivia> {

        private readonly ImmutableArray<TriviaSlot> _trivias;

        internal SyntaxTriviaList(SyntaxToken token, ImmutableArray<TriviaSlot>? trivias, int position) {
            _trivias = trivias ?? ImmutableArray<TriviaSlot>.Empty;
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

        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator<SyntaxTrivia> IEnumerable<SyntaxTrivia>.GetEnumerator() {
            return new EnumeratorImpl(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new EnumeratorImpl(this);
        }

        public struct Enumerator {

            readonly SyntaxTriviaList _list;
            int                       _index;

            internal Enumerator(SyntaxTriviaList list) {
                _list  = list;
                _index = -1;
            }

            public bool MoveNext() {
                int newIndex = _index + 1;
                if (newIndex < _list.Count) {
                    _index = newIndex;
                    return true;
                }

                return false;
            }

            public SyntaxTrivia Current => _list[_index];

            public void Reset() {
                _index = -1;
            }

            public override bool Equals(object obj) {
                throw new NotSupportedException();
            }

            public override int GetHashCode() {
                throw new NotSupportedException();
            }

        }

        class EnumeratorImpl: IEnumerator<SyntaxTrivia> {

            private Enumerator _e;

            internal EnumeratorImpl(in SyntaxTriviaList list) {
                _e = new Enumerator(list);
            }

            public bool MoveNext() {
                return _e.MoveNext();
            }

            public SyntaxTrivia Current => _e.Current;

            void IDisposable.Dispose() {
            }

            object IEnumerator.Current => _e.Current;

            void IEnumerator.Reset() {
                _e.Reset();
            }

        }

    }

}