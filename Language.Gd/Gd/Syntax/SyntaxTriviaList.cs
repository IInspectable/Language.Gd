using System;
using System.Collections;
using System.Collections.Generic;
#region Using Directives

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public struct SyntaxTriviaList: IReadOnlyList<SyntaxTrivia> {

        private readonly SyntaxToken _token;
        private readonly Slot        _slot;

        internal SyntaxTriviaList(SyntaxToken token, Slot trivia, int position) {
            _token   = token;
            _slot    = trivia;
            Position = position;
        }

        public int Position { get; }

        public int Count => _slot == null ? 0 : _slot.IsList ? _slot.SlotCount : 1;

        public SyntaxTrivia this[int index] {
            get {
                if (_slot == null) {
                    throw new ArgumentOutOfRangeException();
                }

                TriviaSlot triviaSlot = null;
                int        position   = Position;
                if (_slot.IsList) {
                    if (index < _slot.SlotCount) {
                        triviaSlot = (TriviaSlot) _slot.GetSlot(index);
                        position   = Position + _slot.GetSlotOffset(index);
                    }
                } else if (index == 0) {
                    triviaSlot = (TriviaSlot) _slot;
                }

                if (triviaSlot == null) {
                    throw new ArgumentOutOfRangeException();
                }

                return triviaSlot.Realize(_token.SyntaxTree, _token, position);
            }
        }

        public TextExtent FullExtent {
            get {
                if (Count == 0) {
                    return TextExtent.Missing;
                }

                return TextExtent.FromBounds(
                    this[0].FullExtent.Start,
                    this[Count - 1].FullExtent.End);
            }
        }

        public TextExtent Extent {
            get {
                if (Count == 0) {
                    return TextExtent.Missing;
                }

                return TextExtent.FromBounds(
                    this[0].Extent.Start,
                    this[Count - 1].Extent.End);
            }
        }

        public bool Any() {
            return _slot != null;
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