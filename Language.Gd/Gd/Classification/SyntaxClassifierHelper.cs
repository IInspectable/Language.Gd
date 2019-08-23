namespace Pharmatechnik.Language.Gd {

    class SyntaxClassifierHelper {

        public static Classification ClassifyToken(SyntaxToken token) {

            if (token.Kind == SyntaxKind.Identifier) {

                if (token.Parent is EventDeclarationSyntax eventDeclaration) {
                    if (eventDeclaration.EventNameToken == token) {
                        return Classification.EventName;
                    }

                    if (eventDeclaration.CallNameToken == token) {
                        return Classification.MethodName;
                    }

                }

                if (token.Parent is ControlSectionBeginSyntax controlSectionBegin) {
                    if (controlSectionBegin.ControlTypeToken == token) {
                        return Classification.ClassName;
                    }
                }

                return Classification.Identifier;
            }

            return ClassifyKind(token.Kind);
        }

        public static Classification ClassifyKind(SyntaxKind syntaxKind) {

            if (SyntaxFacts.IsKeyword(syntaxKind)) {
                return Classification.Keyword;
            }

            if (SyntaxFacts.IsPunctuation(syntaxKind)) {
                return Classification.Punctuation;
            }

            if (SyntaxFacts.IsHotKeyModifier(syntaxKind)) {
                return Classification.StaticSymbol;
            }

            // TODO ClassifyTokenKind
            switch (syntaxKind) {
                case SyntaxKind.MultiLineCommentTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                    return Classification.Comment;
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.NewLineTrivia:
                case SyntaxKind.Eof:
                    return Classification.WhiteSpace;
                case SyntaxKind.SemicolonTrivia:
                    return Classification.Unused;
                case SyntaxKind.Identifier:
                    return Classification.Identifier;
                case SyntaxKind.Integer:
                    return Classification.NumericLiteral;
                case SyntaxKind.String:
                case SyntaxKind.Character:
                    return Classification.StringLiteral;
     
            }

            return Classification.Text;
        }

    }

}