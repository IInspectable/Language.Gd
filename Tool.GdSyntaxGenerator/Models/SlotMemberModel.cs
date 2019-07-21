using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models
{
    class SlotMemberModel {

        public bool IsToken  { get;set;}
        public int SlotIndex { get; set; }
        public string Name   { get; set; }
        public string Cardinality { get;set;}
        public bool IsOptional=> Cardinality=="?";
        public bool IsToN => Cardinality == "*";

        public string ParameterName => Name.ToCamelcase();
        public string FieldName     => $"_{Name.ToCamelcase()}";

    }

}