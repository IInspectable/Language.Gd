#region Using Directives

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace Pharmatechnik.Language.Gd {

    public partial class ChildNodesAndTokenList {

        public class Reversed: IEnumerable<SyntaxNodeOrToken>, IEquatable<Reversed> {

            internal Reversed(SyntaxNode node, int count) {
                Node  = node;
                Count = count;
            }

            internal SyntaxNode Node { get; }

            public int Count { get; }

            public bool Any() {
                return Count != 0;
            }

            public bool Equals(Reversed other) {
                if (ReferenceEquals(null, other)) {
                    return false;
                }

                if (ReferenceEquals(this, other)) {
                    return true;
                }

                return Equals(Node, other.Node) && Count == other.Count;
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) {
                    return false;
                }

                if (ReferenceEquals(this, obj)) {
                    return true;
                }

                if (obj.GetType() != GetType()) {
                    return false;
                }

                return Equals((Reversed) obj);
            }

            public override int GetHashCode() {
                unchecked {
                    return ((Node != null ? Node.GetHashCode() : 0) * 397) ^ Count;
                }
            }

            public static bool operator ==(Reversed left, Reversed right) {
                return Equals(left, right);
            }

            public static bool operator !=(Reversed left, Reversed right) {
                return !Equals(left, right);
            }

            public ReversedEnumerator GetEnumerator() {
                if (Node == null) {
                    return default;
                }

                return new ReversedEnumerator(Node, Count);
            }

            IEnumerator<SyntaxNodeOrToken> IEnumerable<SyntaxNodeOrToken>.GetEnumerator() {
                return new ReversedEnumaratorImpl(Node, Count);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return new ReversedEnumaratorImpl(Node, Count);
            }

            public struct ReversedEnumerator {

                private readonly SyntaxNode _node;
                private readonly int        _count;
                private          int        _index;

                internal ReversedEnumerator(SyntaxNode node, int count) {
                    _node  = node;
                    _count = count;
                    _index = count;

                }

                public bool MoveNext() {
                    return --_index >= 0;
                }

                public SyntaxNodeOrToken Current => ItemInternal(_node, _index);

                public void Reset() {
                    _index = _count;
                }

            }

            class ReversedEnumaratorImpl: IEnumerator<SyntaxNodeOrToken> {

                private ReversedEnumerator _enumerator;

                public ReversedEnumaratorImpl(SyntaxNode node, int count) {
                    _enumerator = new ReversedEnumerator(node, count);

                }

                public SyntaxNodeOrToken Current => _enumerator.Current;

                object IEnumerator.Current => _enumerator.Current;

                public bool MoveNext() {
                    return _enumerator.MoveNext();
                }

                public void Reset() {
                    _enumerator.Reset();
                }

                public void Dispose() {
                }

            }

        }

    }

}