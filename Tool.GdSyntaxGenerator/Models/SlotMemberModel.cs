#region Using Directives

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

#endregion

namespace Tool.GdSyntaxGenerator.Models {

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    class SlotMemberModel {

        public bool   IsToken      { get; set; }
        public bool   IsSyntaxNode => !IsToken;
        public int    SlotIndex    { get; set; }
        public string Name         { get; set; }

        public string SyntaxKind   { get; set; }
        public string Cardinality  { get; set; }
        public bool   IsOptional   => Cardinality == "?";
        public bool   IsRequired   => Cardinality.IsNullOrEmpty();
        public bool   IsToN        => IsZeroOrMore | IsOneOrMore;
        public bool   IsOneOrMore  => Cardinality == "+";
        public bool   IsZeroOrMore => Cardinality == "*";
        public bool   IsLabeled    { get; set; }

        public string ParserRuleName => Name.ToCamelcase();
        public string ParameterName  => NamePluralizedIfToN.ToCamelcase();
        public string PropertyName   => NamePluralizedIfToN;
        public string FieldName      => $"_{NamePluralizedIfToN.ToCamelcase()}";

        string NamePluralizedIfToN {
            get {
                if (!IsToN) {
                    return Name;
                }

                if (Name == "Property") {
                    return "Properties";
                }

                if (Name == "Container") {
                    return "Container";
                }

                return Name + "s";
            }
        }

    }

}