#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    class SyntaxClassifier {

        readonly TextExtent             _extent;
        readonly List<ClassifiedExtent> _result;
        readonly CancellationToken      _cancellationToken;

        SyntaxClassifier(TextExtent extent, List<ClassifiedExtent> result, CancellationToken cancellationToken) {
            _extent            = extent;
            _result            = result;
            _cancellationToken = cancellationToken;

        }

        public static ImmutableArray<ClassifiedExtent> Classify(SyntaxNode node, CancellationToken cancellationToken = default) {

            return Classify(node, node.FullExtent, cancellationToken);
        }

        public static ImmutableArray<ClassifiedExtent> Classify(SyntaxNode node, TextExtent extent, CancellationToken cancellationToken = default) {

            var result = new List<ClassifiedExtent>();
            AddClassifications(node, extent, result, cancellationToken);
            return result.ToImmutableArray();
        }

        public static void AddClassifications(SyntaxNode node, TextExtent extent, List<ClassifiedExtent> result, CancellationToken cancellationToken = default) {

            var worker = new SyntaxClassifier(extent, result, cancellationToken);
            worker.ClassifyNode(node);
        }

        void ClassifyNode(SyntaxNode node) {

            foreach (var token in node.DescendantTokens()) {

                _cancellationToken.ThrowIfCancellationRequested();

                if (token.EndPosition < _extent.Start) {
                    continue;
                }

                if (token.Position > _extent.End) {
                    return;
                }

                ClassifyToken(token);

            }
        }

        void ClassifyToken(SyntaxToken token) {

            ClassifyTriviaList(token.LeadingTrivia);

            AddClassification(token, SyntaxClassifierHelper.ClassifyToken(token));

            ClassifyTriviaList(token.TrailingTrivia);

        }

        void ClassifyTriviaList(SyntaxTriviaList syntaxTriviaList) {

            if (syntaxTriviaList.Count == 0) {
                return;
            }

            // Erst mal alle Trivias überspringen, die nicht im zu klassifizierendem Bereich liegen
            var enumerator = syntaxTriviaList.GetEnumerator();
            while (true) {

                _cancellationToken.ThrowIfCancellationRequested();

                if (!enumerator.MoveNext()) {
                    return;
                }

                if (enumerator.Current.EndPosition > _extent.Start) {
                    // erstes Trivia im zu klassifizierendem Bereich
                    break;
                }
            }

            // Klassifizierung aller Trivias, bis wir den zu klassifizierendem Bereich verlassen
            do {
                _cancellationToken.ThrowIfCancellationRequested();

                var trivia = enumerator.Current;
                if (trivia.Position >= _extent.End) {
                    // Das Ende des zu klassifizierenden Bereichs wurde erreicht
                    return;
                }

                ClassifyTrivia(trivia);

            } while (enumerator.MoveNext());

        }

        void ClassifyTrivia(SyntaxTrivia syntaxTrivia) {

            if (syntaxTrivia.IsSkipedTokenTrivia) {
                AddClassification(syntaxTrivia, Classification.Unused);

            }

            AddClassification(syntaxTrivia, SyntaxClassifierHelper.ClassifyKind(syntaxTrivia.Kind));

        }

        void AddClassification(SyntaxTrivia trivia, Classification classification) {
            AddClassification(trivia.Extent, classification);
        }

        void AddClassification(SyntaxToken token, Classification classification) {
            AddClassification(token.Extent, classification);
        }

        void AddClassification(TextExtent extent, Classification classification) {
            if (ShouldAdd(extent)) {
                _result.Add(new ClassifiedExtent(extent, classification));
            }
        }

        bool ShouldAdd(TextExtent extent) {
            return extent.Length > 0 && _extent.OverlapsWith(extent);
        }

    }

}