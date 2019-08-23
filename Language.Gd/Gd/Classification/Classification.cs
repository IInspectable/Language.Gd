namespace Pharmatechnik.Language.Gd {

    public enum Classification {

        Comment,
        Unused,
        Identifier,
        Keyword,
        NumericLiteral,
        Operator,
        StringLiteral,
        WhiteSpace,
        Text,

        StaticSymbol,
        Punctuation,
        VerbatimStringLiteral,
        ClassName,
        PropertyName,
        EventName,
        NamespaceName,
        MethodName,
        InterfaceName,
        ConstantName,

    }

/*
  public static readonly Classification Comment        = new Classification("comment");
        public static readonly Classification Unused         = new Classification("excluded code");
        public static readonly Classification Identifier     = new Classification("identifier");
        public static readonly Classification Keyword        = new Classification("keyword");
        public static readonly Classification NumericLiteral = new Classification("number");
        public static readonly Classification Operator       = new Classification("operator");
        public static readonly Classification StringLiteral  = new Classification("string");
        public static readonly Classification WhiteSpace     = new Classification("whitespace");
        public static readonly Classification Text           = new Classification("text");

        public static readonly Classification StaticSymbol          = new Classification("static symbol");
        public static readonly Classification Punctuation           = new Classification("punctuation");
        public static readonly Classification VerbatimStringLiteral = new Classification("string - verbatim");
        public static readonly Classification ClassName             = new Classification("class name");
        public static readonly Classification PropertyName          = new Classification("property name");
        public static readonly Classification EventName             = new Classification("event name");
        public static readonly Classification NamespaceName         = new Classification("namespace name");
        public static readonly Classification MethodName            = new Classification("method name");
        public static readonly Classification InterfaceName         = new Classification("interface name");
        public static readonly Classification ConstantName          = new Classification("constant name");

 */

}