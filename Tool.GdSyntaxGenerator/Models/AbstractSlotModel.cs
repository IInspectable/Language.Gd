
using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models
{
    class AbstractSlotModel {

        public AbstractSlotModel(ParserRule rule) {

            RuleName=rule.Name.ToPascalcase();

            // TODO richtige Basisklasse?
            SyntaxBaseClassName = "SyntaxNode";
            SyntaxClassName     = $"{rule.Name.ToPascalcase()}Syntax";
            SlotClassName       = $"{rule.Name.ToPascalcase()}SyntaxSlot";
        }

        public string RuleName { get;}

        public string SyntaxBaseClassName { get; }
        public string SyntaxClassName     { get; }
        public string SlotClassName       { get; }

    }

}