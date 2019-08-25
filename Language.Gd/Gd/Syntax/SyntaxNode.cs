#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public abstract class SyntaxNode {

        private protected SyntaxNode(SyntaxTree syntaxTree, SyntaxSlot slot, SyntaxNode parent, int position) {

            SyntaxTree = syntaxTree ?? throw new ArgumentNullException(nameof(syntaxTree));
            Slot       = slot       ?? throw new ArgumentNullException(nameof(slot));
            Parent     = parent;
            Position   = position;

        }

        internal SyntaxSlot Slot { get; }

        public SyntaxTree SyntaxTree { get; }

        public bool IsMissing => Slot.IsMissing;

        [CanBeNull]
        public SyntaxNode Parent { get; }

        public SyntaxKind Kind => Slot.Kind;

        public int        ExtentStart => Position + Slot.GetLeadingTriviaWidth();
        public TextExtent Extent      => Slot.GetExtent(Position);
        public TextExtent FullExtent  => TextExtent.FromBounds(start: Position, end: EndPosition);

        internal int Position    { get; }
        internal int EndPosition => Position + Slot.FullLength;

        internal int Length     => Slot.Length;
        internal int FullLength => Slot.FullLength;

        internal bool IsList    => Slot.IsList;
        internal int  SlotCount => Slot.SlotCount;

        public override string ToString() {
            return $"N: {Kind}: {SyntaxTree.SourceText.Substring(Extent)}";
        }

        // TODO Descendants Childs etc...

        // TODO LastToken FirstToken

        public SyntaxToken FindToken(int position) {

            if (TryGetEofAt(out var eof)) {
                return eof;
            }

            var child = ChildThatContainsPosition(position);
            if (child.IsToken) {
                return child.AsToken();
            }

            return child.AsNode().FindTokenImpl(position);

            bool TryGetEofAt(out SyntaxToken eofToken) {

                if (position == EndPosition) {
                    if (this is GuiDescriptionSyntax guiDescriptionSyntax) {
                        eofToken = guiDescriptionSyntax.EofToken;
                        Debug.Assert(eof.EndPosition == position);
                        return true;
                    }
                }

                eofToken = default;
                return false;
            }
        }

        private SyntaxToken FindTokenImpl(int position) {

            var child = ChildThatContainsPosition(position);
            if (child.IsToken) {
                return child.AsToken();
            }

            return child.AsNode().FindToken(position);
        }

        // TODO Überladung mit Textextent?
        public IEnumerable<SyntaxToken> DescendantTokens() {
            return DescendantTokensImpl();
        }

        IEnumerable<SyntaxToken> DescendantTokensImpl() {

            foreach (var child in ChildNodesAndTokens()) {

                if (child.IsToken) {
                    yield return child.AsToken();
                }

                if (child.IsNode) {
                    foreach (var token in child.AsNode().DescendantTokens()) {
                        yield return token;
                    }
                }

            }
        }

        public IEnumerable<SyntaxNode> ChildNodes() {
            foreach (var nodeOrToken in ChildNodesAndTokens()) {
                if (nodeOrToken.IsNode) {
                    yield return nodeOrToken.AsNode();
                }
            }
        }

        public IEnumerable<SyntaxNode> DescendantNodes() {
            return DescendantNodesImpl(FullExtent, includeSelf: false);
        }

        public IEnumerable<SyntaxNode> DescendantNodes(TextExtent extent) {
            return DescendantNodesImpl(extent, includeSelf: false);
        }

        public IEnumerable<SyntaxNode> DescendantNodesAndSelf() {
            return DescendantNodesImpl(FullExtent, includeSelf: true);
        }

        public IEnumerable<SyntaxNode> DescendantNodesAndSelf(TextExtent extent) {
            return DescendantNodesImpl(extent, includeSelf: true);
        }

        IEnumerable<SyntaxNode> DescendantNodesImpl(TextExtent extent, bool includeSelf) {

            if (includeSelf && IsInExtent(FullExtent)) {
                yield return this;
            }

            foreach (var child in ChildNodesAndTokens()) {

                if (child.IsNode) {
                    foreach (var node in child.AsNode().DescendantNodesAndSelf()) {
                        if (IsInExtent(node.FullExtent)) {
                            yield return node;
                        }

                    }
                }

            }

            bool IsInExtent(TextExtent childExtent) {
                return extent.OverlapsWith(childExtent)
                       // special case for zero-width tokens (OverlapsWith never returns true for these)
                    || childExtent.Length == 0 && extent.IntersectsWith(childExtent);
            }
        }

        public ChildNodesAndTokenList ChildNodesAndTokens() => new ChildNodesAndTokenList(this);

        public virtual SyntaxNodeOrToken ChildThatContainsPosition(int position) {
            if (!FullExtent.Contains(position)) {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            SyntaxNodeOrToken childNodeOrToken = ChildNodesAndTokenList.ChildThatContainsPosition(this, position);
            Debug.Assert(childNodeOrToken.FullExtent.Contains(position), "ChildThatContainsPosition's return value does not contain the requested position.");
            return childNodeOrToken;
        }

        /// <summary>
        /// Gets a list of ancestor nodes
        /// </summary>
        public IEnumerable<SyntaxNode> Ancestors() {
            return Parent?.AncestorsAndSelf() ?? Enumerable.Empty<SyntaxNode>();
        }

        /// <summary>
        /// Gets a list of ancestor nodes (including this node) 
        /// </summary>
        public IEnumerable<SyntaxNode> AncestorsAndSelf() {
            for (var node = this; node != null; node = node.Parent) {
                yield return node;
            }
        }

        /// <summary>
        /// Determines if the specified node is a descendant of this node.
        /// Returns true for current node.
        /// </summary>
        public bool Contains(SyntaxNode node) {
            if (node == null || !FullExtent.Contains(node.FullExtent)) {
                return false;
            }

            while (node != null) {
                if (node == this) {
                    return true;
                }

                node = node.Parent;
            }

            return false;
        }

        // TODO Hier Evtl. SyntaxKind reinkippen wegen Missing Tokens
        private protected SyntaxToken GetSyntaxToken(TokenSlot slot, int index) {

            return slot.Realize(SyntaxTree, this, GetChildPosition(index));

        }

        [CanBeNull]
        private protected SyntaxListNode GetSyntaxNode(ref SyntaxListNode field, [CanBeNull] SyntaxSlotList slot, int index) {

            if (slot == null) {
                return null;
            }

            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, (SyntaxListNode) slot.Realize(SyntaxTree, this, GetChildPosition(index)), null);
                result = field;
            }

            return result;
        }

        [CanBeNull]
        private protected TNode GetSyntaxNode<TNode>(ref TNode field, [CanBeNull] SyntaxSlot slot, int index) where TNode : SyntaxNode {

            if (slot == null) {
                return null;
            }

            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, (TNode) slot.Realize(SyntaxTree, this, GetChildPosition(index)), null);
                result = field;
            }

            return result;
        }

        private protected SyntaxNode GetSyntaxNodeElement(ref SyntaxNode element, int index) {
            Debug.Assert(IsList);

            var result = element;

            if (result == null) {
                var slot = (SyntaxSlot) Slot.GetSlot(index);
                // passing list's parent
                Interlocked.CompareExchange(ref element, slot.Realize(SyntaxTree, Parent, GetChildPosition(index)), null);
                result = element;
            }

            return result;
        }

        internal abstract SyntaxNode GetCachedSyntaxNode(int index);
        internal abstract SyntaxNode GetSyntaxNode(int index);

        internal virtual int GetChildPosition(int index) {
            int offset = 0;
            var slot   = Slot;
            while (index > 0) {
                index--;

                // Wenn es den Knoten bereits gibt, kann direkt dessen EndPosition
                // verwendet werden, und es muss nicht weiter bis zum ersten Slot
                // durchiteriert werden.
                var prevSibling = GetCachedSyntaxNode(index);
                if (prevSibling != null) {
                    return prevSibling.EndPosition + offset;
                }

                var slotChild = slot.GetSlot(index);
                if (slotChild != null) {
                    offset += slotChild.FullLength;
                }
            }

            return Position + offset;
        }

    }

}