#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    class TriviaSpanClassifierWorker: ClassifierWorker {

        public TriviaSpanClassifierWorker(TextExtent extent, List<ClassifiedExtent> result, CancellationToken cancellationToken): base(extent, result, cancellationToken) {
        }

        protected override void ClassifyToken(SyntaxToken token) {
            AddClassification(token, GdClassification.TokenSpan);
        }

        protected override void ClassifyTrivia(SyntaxTrivia syntaxTrivia, bool isLeadingTrivia) {
            AddClassification(syntaxTrivia, isLeadingTrivia ? GdClassification.LeadingTriviaSpan : GdClassification.TrailingTriviaSpan);
        }

    }

    public class TriviaSpanClassifier {

        public static ImmutableArray<ClassifiedExtent> Classify(SyntaxNode node, CancellationToken cancellationToken = default) {

            return Classify(node, node.FullExtent, cancellationToken);
        }

        public static ImmutableArray<ClassifiedExtent> Classify(SyntaxNode node, TextExtent extent, CancellationToken cancellationToken = default) {

            var result = new List<ClassifiedExtent>();
            AddClassifications(node, extent, result, cancellationToken);
            return result.ToImmutableArray();
        }

        public static void AddClassifications(SyntaxNode node, TextExtent extent, List<ClassifiedExtent> result, CancellationToken cancellationToken = default) {

            var worker = new TriviaSpanClassifierWorker(extent, result, cancellationToken);
            worker.ClassifyNode(node);
        }

    }

}