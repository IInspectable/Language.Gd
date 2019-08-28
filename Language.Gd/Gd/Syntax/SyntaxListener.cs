namespace Pharmatechnik.Language.Gd {

    public abstract class SyntaxListener: SyntaxVisitor {

        protected SyntaxListener(SyntaxListenerDepth depth = SyntaxListenerDepth.Node) {
            Depth = depth;
        }

        protected SyntaxListenerDepth Depth { get; }

        protected override void DefaultVisit(SyntaxNode node) {

            if (node == null) {
                return;
            }

            foreach (var child in node.ChildNodesAndTokens()) {
                if (child.IsNode) {
                    if (Depth >= SyntaxListenerDepth.Node) {
                        Visit(child.AsNode());
                    }
                } else if (child.IsToken) {
                    if (Depth >= SyntaxListenerDepth.Token) {
                        VisitTokenImpl(child.AsToken());
                    }
                }
            }
        }

        void VisitTokenImpl(SyntaxToken token) {

            if (Depth >= SyntaxListenerDepth.Trivia) {
                VisitLeadingTrivia(token);
            }

            VisitToken(token);

            if (Depth >= SyntaxListenerDepth.Trivia) {
                VisitTrailingTrivia(token);
            }
        }

        protected virtual void VisitToken(SyntaxToken token) {

        }

        void VisitLeadingTrivia(SyntaxToken token) {
            if (token.HasLeadingTrivia) {
                foreach (var trivia in token.LeadingTrivia) {
                    VisitTrivia(trivia);
                }
            }
        }

        void VisitTrailingTrivia(SyntaxToken token) {
            if (token.HasTrailingTrivia) {
                foreach (var trivia in token.TrailingTrivia) {
                    VisitTrivia(trivia);
                }
            }
        }

        protected virtual void VisitTrivia(SyntaxTrivia trivia) {

        }

    }

}