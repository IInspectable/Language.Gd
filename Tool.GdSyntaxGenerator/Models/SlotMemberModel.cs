using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models {

    class SlotMemberModel {

        public bool   IsToken      { get; set; }
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

        public string ParameterName => Name.ToCamelcase();
        public string FieldName     => $"_{Name.ToCamelcase()}";

    }

}