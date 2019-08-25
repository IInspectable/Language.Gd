#region Using Directives

using System.Collections.Generic;
using System.Threading;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    abstract class ClassifierWorker {

        readonly TextExtent             _extent;
        readonly List<ClassifiedExtent> _result;
        readonly CancellationToken      _cancellationToken;

        protected ClassifierWorker(TextExtent extent, List<ClassifiedExtent> result, CancellationToken cancellationToken) {
            _extent            = extent;
            _result            = result;
            _cancellationToken = cancellationToken;

        }

        public void ClassifyNode(SyntaxNode node) {

            foreach (var token in node.DescendantTokens()) {

                _cancellationToken.ThrowIfCancellationRequested();

                if (token.EndPosition < _extent.Start) {
                    continue;
                }

                if (token.Position > _extent.End) {
                    return;
                }

                ClassifyTokenImpl(token);

            }
        }

        protected abstract void ClassifyToken(SyntaxToken token);
        protected abstract void ClassifyTrivia(SyntaxTrivia syntaxTrivia, bool isLeadingTrivia);

        protected void AddClassification(SyntaxTrivia trivia, GdClassification classification) {
            AddClassification(trivia.Extent, classification);
        }

        protected void AddClassification(SyntaxToken token, GdClassification classification) {
            AddClassification(token.Extent, classification);
        }

        protected void AddClassification(TextExtent extent, GdClassification classification) {
            if (ShouldAdd()) {
                _result.Add(new ClassifiedExtent(extent, classification));
            }

            bool ShouldAdd() {
                return extent.Length > 0 && _extent.OverlapsWith(extent);
            }
        }

        void ClassifyTokenImpl(SyntaxToken token) {

            ClassifyTriviaList(token.LeadingTrivia, isLeadingTrivia: true);
            ClassifyToken(token);
            ClassifyTriviaList(token.TrailingTrivia, isLeadingTrivia: false);

        }

        void ClassifyTriviaList(SyntaxTriviaList syntaxTriviaList, bool isLeadingTrivia) {

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

                ClassifyTrivia(trivia, isLeadingTrivia);

            } while (enumerator.MoveNext());

        }

    }

}