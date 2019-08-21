namespace Pharmatechnik.Language.Gd {

    public class SyntaxFacts {

        public static bool IsKeyword(SyntaxKind kind) {
            return kind >= SyntaxKind.Using && kind <= SyntaxKind.ContextMenu;
        }

    }

}