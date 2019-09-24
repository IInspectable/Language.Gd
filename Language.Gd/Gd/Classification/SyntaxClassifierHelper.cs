namespace Pharmatechnik.Language.Gd {

    static class SyntaxClassifierHelper {

        public static GdClassification ClassifyToken(SyntaxToken token) {

            if (token.Kind == SyntaxKind.Identifier) {

                if (token.Parent is EventDeclarationSyntax eventDeclaration) {
                    if (eventDeclaration.EventNameToken == token) {
                        return GdClassification.EventName;
                    }

                    if (eventDeclaration.CallNameToken == token) {
                        return GdClassification.MethodName;
                    }

                    if (eventDeclaration.CallTypeToken == token) {
                        return GdClassification.CallType;
                    }

                }

                if (token.Parent is HotkeyDeclarationSyntax hotkeyDeclaration) {

                    if (hotkeyDeclaration.HotKeyNameToken == token) {
                        return GdClassification.StaticSymbol;
                    }

                    if (hotkeyDeclaration.CallNameToken == token) {
                        return GdClassification.MethodName;
                    }

                    if (hotkeyDeclaration.CallTypeToken == token) {
                        return GdClassification.CallType;
                    }

                }

                if (token.Parent is PropertyAddAssignSyntax propertyAddAssign) {

                    if (propertyAddAssign.CallNameToken == token) {
                        return GdClassification.MethodName;
                    }

                    if (propertyAddAssign.CallTypeToken == token) {
                        return GdClassification.CallType;
                    }

                }

                // Dialog Section
                if (token.Parent is DialogSectionBeginSyntax dialogSectionBegin) {

                    if (dialogSectionBegin.DialogIdentifierToken == token) {
                        return GdClassification.ClassName;
                    }

                }

                if (token.Parent is DialogSectionEndSyntax dialogSectionEnd) {

                    if (dialogSectionEnd.DialogIdentifierToken == token) {
                        return GdClassification.ClassName;
                    }

                }

                // Form Section
                if (token.Parent is FormSectionBeginSyntax formSectionBegin) {

                    if (formSectionBegin.FormIdentifierToken == token) {
                        return GdClassification.ClassName;
                    }

                }

                if (token.Parent is FormSectionEndSyntax formSectionEnd) {

                    if (formSectionEnd.FormIdentifierToken == token) {
                        return GdClassification.ClassName;
                    }

                }

                // User Control
                if (token.Parent is UserControlSectionBeginSyntax userControlSectionBegin) {

                    if (userControlSectionBegin.UserControlIdentifierToken == token) {
                        return GdClassification.ClassName;
                    }

                }

                if (token.Parent is UserControlSectionEndSyntax userControlSectionEnd) {

                    if (userControlSectionEnd.UserControlIdentifierToken == token) {
                        return GdClassification.ClassName;
                    }

                }

                // Control Section
                if (token.Parent is ControlSectionBeginSyntax controlSectionBegin) {
                    if (controlSectionBegin.ControlTypeToken == token) {
                        return GdClassification.ClassName;
                    }
                }

                return GdClassification.Identifier;
            }

            return ClassifyKind(token.Kind);
        }

        public static GdClassification ClassifyKind(SyntaxKind syntaxKind) {

            if (SyntaxFacts.IsKeyword(syntaxKind)) {
                return GdClassification.Keyword;
            }

            if (SyntaxFacts.IsPunctuation(syntaxKind)) {
                return GdClassification.Punctuation;
            }

            if (SyntaxFacts.IsHotKeyModifier(syntaxKind)) {
                return GdClassification.StaticSymbol;
            }

            switch (syntaxKind) {
                case SyntaxKind.MultiLineCommentTrivia:
                case SyntaxKind.SingleLineCommentTrivia:
                    return GdClassification.Comment;
                case SyntaxKind.WhitespaceTrivia:
                case SyntaxKind.NewLineTrivia:
                case SyntaxKind.Eof:
                    return GdClassification.WhiteSpace;
                case SyntaxKind.SemicolonTrivia:
                    return GdClassification.Skiped;
                case SyntaxKind.Identifier:
                    return GdClassification.Identifier;
                case SyntaxKind.Integer:
                    return GdClassification.NumericLiteral;
                case SyntaxKind.String:
                case SyntaxKind.Character:
                    return GdClassification.StringLiteral;

            }

            return GdClassification.Text;
        }

    }

}