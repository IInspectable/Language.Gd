#region Using Directives

using System.Linq;

using JetBrains.Annotations;

#endregion

namespace Pharmatechnik.Language.Gd {

    static class SyntaxNodeExtensions {

        [CanBeNull]
        public static TNode AncestorOfType<TNode>(this SyntaxNode node) where TNode : SyntaxNode {
            return node?.Ancestors().OfType<TNode>().FirstOrDefault();
        }

        public static bool HasAncestorOfType<TNode>(this SyntaxNode node) where TNode : SyntaxNode {
            return HasAncestorOfType<TNode>(node, out _);
        }

        public static bool HasAncestorOfType<TNode>(this SyntaxNode node, out TNode ancestor) where TNode : SyntaxNode {
            ancestor = node?.Ancestors().OfType<TNode>().FirstOrDefault();
            return ancestor != null;
        }

    }

}