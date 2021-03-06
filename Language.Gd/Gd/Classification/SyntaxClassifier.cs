﻿#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    class SyntaxClassifierWorker: ClassifierWorker {

        public SyntaxClassifierWorker(TextExtent extent, List<ClassifiedExtent> result, CancellationToken cancellationToken): base(extent, result, cancellationToken) {
        }

        protected override void ClassifyToken(SyntaxToken token) {
            AddClassification(token, SyntaxClassifierHelper.ClassifyToken(token));
        }

        protected override void ClassifyTrivia(SyntaxTrivia syntaxTrivia, bool isLeadingTrivia) {

            var classification = SyntaxClassifierHelper.ClassifyKind(syntaxTrivia.Kind);

            if (syntaxTrivia.IsSkipedTokenTrivia) {
                AddClassification(syntaxTrivia, GdClassification.Skiped);

                if (classification == GdClassification.Text) {
                    classification = GdClassification.Unknown;
                }

            }

            AddClassification(syntaxTrivia, classification);
        }

    }

    public class SyntaxClassifier {

        public static ImmutableArray<ClassifiedExtent> Classify(SyntaxNode node, CancellationToken cancellationToken = default) {

            return Classify(node, node.FullExtent, cancellationToken);
        }

        public static ImmutableArray<ClassifiedExtent> Classify(SyntaxNode node, TextExtent extent, CancellationToken cancellationToken = default) {

            var result = new List<ClassifiedExtent>();
            AddClassifications(node, extent, result, cancellationToken);
            return result.ToImmutableArray();
        }

        public static void AddClassifications(SyntaxNode node, TextExtent extent, List<ClassifiedExtent> result, CancellationToken cancellationToken = default) {

            var worker = new SyntaxClassifierWorker(extent, result, cancellationToken);
            worker.ClassifyNode(node);
        }

    }

}