#region Using Directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Pharmatechnik.Language.Gd {

    static class SyntaxNodeExtensions {
        
        public static bool HasAncestorOfType<TNode>(this SyntaxNode node) where TNode : SyntaxNode {
            return HasAncestorOfType<TNode>(node, out _);
        }

        public static bool HasAncestorOfType<TNode>(this SyntaxNode node, out TNode ancestor) where TNode : SyntaxNode {
            ancestor = AncestorsOfType<TNode>(node).FirstOrDefault();
            return ancestor != null;
        }

        public static bool HasAncestorOrSelfOfType<TNode>(this SyntaxNode node) where TNode : SyntaxNode {
            return HasAncestorOrSelfOfType<TNode>(node, out _);
        }

        public static bool HasAncestorOrSelfOfType<TNode>(this SyntaxNode node, out TNode ancestor) where TNode : SyntaxNode {
            ancestor = AncestorsAndSelfOfType<TNode>(node).FirstOrDefault();
            return ancestor != null;
        }
        
        public static IEnumerable<TNode> AncestorsOfType<TNode>(this SyntaxNode node) where TNode : SyntaxNode {
            return node?.Ancestors().OfType<TNode>() ?? Enumerable.Empty<TNode>();
        }

        public static IEnumerable<TNode> AncestorsAndSelfOfType<TNode>(this SyntaxNode node) where TNode : SyntaxNode {
            return node?.AncestorsAndSelf().OfType<TNode>() ?? Enumerable.Empty<TNode>();
        }

    }

}