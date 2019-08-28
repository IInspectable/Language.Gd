using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models {

    class AbstractSlotModel {

        public AbstractSlotModel(ParserRule rule, string baseNamespace) {

            RawRuleName   = rule.Name;
            RuleName      = rule.Name.ToPascalcase();
            BaseNamespace = baseNamespace;
        }

        public string RuleName      { get; }
        public string RawRuleName   { get; }
        public string BaseNamespace { get; }

        public string RuleNameParameter => RuleName.ToCamelcase();

    }

}