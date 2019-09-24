#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public struct SyntaxList<TSyntax>: IReadOnlyList<TSyntax> where TSyntax : SyntaxNode {

        private readonly SyntaxNode _node;

        SyntaxList(SyntaxNode slot) {
            _node = slot;
        }

        internal static SyntaxList<TSyntax> CreateList(SyntaxNode node) {
            return new SyntaxList<TSyntax>(node);
        }

        public int Count => _node == null ? 0 : _node.IsList ? _node.SlotCount : 1;

        public TSyntax this[int index] {
            get {
                if (_node == null) {
                    throw new ArgumentOutOfRangeException();
                }

                if (_node.IsList) {
                    if (index < _node.SlotCount) {
                        return (TSyntax) _node.GetSyntaxNode(index);
                    }
                } else if (index == 0) {
                    return (TSyntax) _node;
                }

                throw new ArgumentOutOfRangeException();
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
            return _node != null;
        }

        public Enumerator GetEnumerator() {
            return new Enumerator(this);
        }

        IEnumerator<TSyntax> IEnumerable<TSyntax>.GetEnumerator() {
            return new EnumeratorImpl(this);
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return new EnumeratorImpl(this);
        }

        public struct Enumerator {

            readonly SyntaxList<TSyntax> _list;
            int                          _index;

            internal Enumerator(SyntaxList<TSyntax> list) {
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

            public TSyntax Current => _list[_index];

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

        class EnumeratorImpl: IEnumerator<TSyntax> {

            private Enumerator _e;

            internal EnumeratorImpl(in SyntaxList<TSyntax> list) {
                _e = new Enumerator(list);
            }

            public bool MoveNext() {
                return _e.MoveNext();
            }

            public TSyntax Current => _e.Current;

            void IDisposable.Dispose() {
            }

            object IEnumerator.Current => _e.Current;

            void IEnumerator.Reset() {
                _e.Reset();
            }

        }

    }

}