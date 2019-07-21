using System.Linq;
using System.Collections.Generic;

namespace Tool.GdSyntaxGenerator.Models
{

    class SlotModels {

        public SlotModels(GrammarInfo grammarInfo) {

            // Wenn eine Regel aus mehr als einer Alternative besteht, dann wird diese zur Basisklasse
            // für die konkreten Syntaxen
            var baseRules = new Dictionary<string, string>();
            foreach (var parserRule in grammarInfo.Rules.Where(rule => rule.Alternatives.Count > 1)) {
                AbstractSlots.Add(new AbstractSlotModel(parserRule));
                foreach (var alternative in parserRule.Alternatives) {
                    baseRules[alternative.Elements[0].Name] = parserRule.Name;
                }
            }

            // Aus Regeln mir genau einer "Alternative" werden zu konkreten Syntaxen
            foreach (var parserRule in grammarInfo.Rules.Where(rule => rule.Alternatives.Count == 1)) {
                baseRules.TryGetValue(parserRule.Name, out var baseRule);
                Slots.Add(new SlotModel(parserRule, baseRule));
            }

        }
              

        public List<SlotModel>         Slots         { get; } = new List<SlotModel>();
        public List<AbstractSlotModel> AbstractSlots { get; } = new List<AbstractSlotModel>();

    }

}