using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models {

    class AbstractSlotModel {

        public AbstractSlotModel(ParserRule rule) {

            RawRuleName = rule.Name;
            RuleName    = rule.Name.ToPascalcase();
        }

        public string RuleName    { get; }
        public string RawRuleName { get; }

    }

}