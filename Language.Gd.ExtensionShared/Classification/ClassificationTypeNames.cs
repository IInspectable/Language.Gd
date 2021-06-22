namespace Pharmatechnik.Language.Gd.Extension.Classification {

    using CSharpClassificationTypeNames = Microsoft.CodeAnalysis.Classification.ClassificationTypeNames;

    static class ClassificationTypeNames {

        public const string EventName      = CSharpClassificationTypeNames.EventName;
        public const string MethodName     = CSharpClassificationTypeNames.MethodName;
        public const string NumericLiteral = CSharpClassificationTypeNames.NumericLiteral;
        public const string Keyword        = CSharpClassificationTypeNames.Keyword;
        public const string Comment        = CSharpClassificationTypeNames.Comment;
        public const string Identifier     = CSharpClassificationTypeNames.Identifier;
        public const string StringLiteral  = CSharpClassificationTypeNames.StringLiteral;
        public const string Punctuation    = CSharpClassificationTypeNames.Punctuation;
        public const string ClassName      = CSharpClassificationTypeNames.ClassName;
        public const string Operator       = CSharpClassificationTypeNames.Operator;
        public const string Text           = CSharpClassificationTypeNames.Text;
        public const string WhiteSpace     = CSharpClassificationTypeNames.WhiteSpace;

        public const string VerbatimStringLiteral = CSharpClassificationTypeNames.VerbatimStringLiteral;
        public const string PropertyName          = CSharpClassificationTypeNames.PropertyName;
        public const string NamespaceName         = CSharpClassificationTypeNames.NamespaceName;
        public const string ConstantName          = CSharpClassificationTypeNames.ConstantName;

        // TODO ClassificationTypeNames

        public const string StaticSymbol = "Gd StaticSymbol"; // CSharpClassificationTypeNames.StaticSymbol;
        public const string Unknown      = "Gd Unknown";
        public const string Skiped       = "Gd Skiped Token";
        public const string Underline    = "Gd Underline";
        public const string CallType     = "Gd CallType";

        public const string LeadingTriviaSpan  = "Gd LeadingTriviaSpan";
        public const string TokenSpan          = "Gd TokenSpan";
        public const string TrailingTriviaSpan = "Gd TrailingTriviaSpan";

    }

}