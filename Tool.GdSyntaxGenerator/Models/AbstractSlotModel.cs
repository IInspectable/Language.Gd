#region Using Directives

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

#endregion

namespace Tool.GdSyntaxGenerator.Models {

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
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