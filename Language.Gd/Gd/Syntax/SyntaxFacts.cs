#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;

using Pharmatechnik.Language.Gd.Antlr;

#endregion

namespace Pharmatechnik.Language.Gd {

    public static partial class SyntaxFacts {

        public static bool IsKeyword(SyntaxKind kind) {
            return kind >= SyntaxKind.Using && kind <= SyntaxKind.ContextMenu;
        }

        public static readonly ImmutableHashSet<string> Keywords = GetKeywords().ToImmutableHashSet();

        static IEnumerable<string> GetKeywords() {
            foreach (var kw in Enum.GetValues(typeof(SyntaxKind))
                                   .OfType<SyntaxKind>()
                                   .Where(IsKeyword)) {
                yield return GetLiteralName(kw);
            }
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

        static string GetLiteralName(SyntaxKind tokenType) {
            return GdGrammar.DefaultVocabulary.GetLiteralName((int) tokenType).Trim('\'');
        }

    }

}