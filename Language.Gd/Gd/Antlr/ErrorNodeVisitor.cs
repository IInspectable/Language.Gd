using System.Collections.Generic;

using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

namespace Pharmatechnik.Language.Gd.Antlr {

    class ErrorNodeVisitor: GdGrammarBaseVisitor<bool> {

       
        public List<IErrorNode> ErrorNodes { get; } = new List<IErrorNode>();

        public override bool VisitErrorNode([NotNull] IErrorNode node) {
            ErrorNodes.Add(node);
            return base.VisitErrorNode(node);
        }

    }

}