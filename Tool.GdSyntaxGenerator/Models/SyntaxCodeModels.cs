using System.Linq;
using System.Collections.Generic;

namespace Tool.GdSyntaxGenerator.Models
{

    class SyntaxCodeModels {

        public SyntaxCodeModels(TokenInfo tokenInfo,
                                GrammarInfo grammarInfo) {

            Dictionary<string, string> baseRules = new Dictionary<string, string>();
            foreach (var parserRule in grammarInfo.Rules.Where(rule => rule.Alternatives.Count > 1)) {
                AbstractSlots.Add(new AbstractSlotModel(parserRule));
                foreach (var alternative in parserRule.Alternatives) {
                    baseRules[alternative.Elements[0].Name] = parserRule.Name;
                }
            }

            foreach (var parserRule in grammarInfo.Rules.Where(rule => rule.Alternatives.Count == 1)) {
                baseRules.TryGetValue(parserRule.Name, out var baseRule);
                Slots.Add(new SlotModel(parserRule, baseRule));
            }

        }
              

        public List<SlotModel>         Slots         { get; } = new List<SlotModel>();
        public List<AbstractSlotModel> AbstractSlots { get; } = new List<AbstractSlotModel>();

    }

}