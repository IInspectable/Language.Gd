namespace Pharmatechnik.Language.Gd {

    /// <summary>
    /// Es besteht nicht notwendigerweise eine 1:1 Bezihung zwischen einem
    /// Textbereich und einer Klassifizierung. Ein Textbereich kann auch mehr
    /// als eine Klassifizierung zugeweisen bekommen, z.B. {Skiped, Keyword},
    /// wenn ein Keyword an der falschen Stelle auftaucht.
    /// </summary>
    public enum GdClassification {

        Unknown,
        Comment,
        Skiped,
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
        CallType,

        // Zum visuellen Debuggen
        LeadingTriviaSpan,
        TokenSpan,
        TrailingTriviaSpan

    }

}