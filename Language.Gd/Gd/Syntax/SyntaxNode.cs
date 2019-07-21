﻿using System;
using System.Threading;

using JetBrains.Annotations;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd {

    public abstract class SyntaxNode {

        protected SyntaxNode(SyntaxTree syntaxTree, SyntaxSlot slot, SyntaxNode parent) {
            SyntaxTree = syntaxTree ?? throw new ArgumentNullException(nameof(syntaxTree));
            Slot       = slot       ?? throw new ArgumentNullException(nameof(slot));
            Parent     = parent;

        }

        public SyntaxTree SyntaxTree { get; }

        [CanBeNull]
        public SyntaxNode Parent { get; }

        public TextExtent Extent => Slot.Extent;
        public SyntaxKind Kind   => Slot.Kind;

        internal SyntaxSlot Slot { get; }

        protected SyntaxNode GetSyntaxNode(ref SyntaxNode field, SyntaxSlot slot) {
            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, slot.Realize(SyntaxTree, this), null);
                result = field;
            }

            return result;
        }

        protected T GetSyntaxNode<T>(ref T field, SyntaxSlot slot) where T : SyntaxNode {
            var result = field;
            if (result == null) {
                Interlocked.CompareExchange(ref field, (T) slot.Realize(SyntaxTree, this), null);
                result = field;
            }

            return result;
        }

    }


}