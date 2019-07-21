using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models {

    class AbstractSlotModel {

        public AbstractSlotModel(ParserRule rule) {

            RuleName = rule.Name.ToPascalcase();
        }

        public string RuleName { get; }

    }

}