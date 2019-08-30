namespace Pharmatechnik.Language.Gd {

    public static partial class SyntaxFacts {

        public static bool IsKeyword(SyntaxKind kind) {
            return kind >= SyntaxKind.Using && kind <= SyntaxKind.ContextMenu;
        }

        public static bool IsPunctuation(SyntaxKind kind) {
            return kind >= SyntaxKind.OpenBrace && kind <= SyntaxKind.MinusEquals;
        }

        public static bool IsHotKeyModifier(SyntaxKind kind) {
            return kind >= SyntaxKind.PlusCtrl && kind <= SyntaxKind.MinusShift;
        }

        public static readonly string SingleLineComment = "//";
        public static readonly string BlockCommentStart = "/*";
        public static readonly string BlockCommentEnd   = "*/";

    }

}