using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Language.Text;

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

    class SlotModel: AbstractSlotModel {

        public string BaseRuleName { get; }
        public string SyntaxKind          { get; }

        public List<SlotMemberModel> Slots { get; } = new List<SlotMemberModel>();


        public List<SlotMemberModel> TokenSlots  { get; } = new List<SlotMemberModel>();
        public List<SlotMemberModel> SyntaxSlots { get; } = new List<SlotMemberModel>();

        public List<SlotMemberModel> SyntaxTokens { get; } = new List<SlotMemberModel>();
        public List<SlotMemberModel> SyntaxNodes  { get; } = new List<SlotMemberModel>();

        public SlotModel(ParserRule rule, string baseRule): base(rule) {

            if (rule.Alternatives.Count != 1) {
                throw new ArgumentException();
            }

            BaseRuleName= baseRule!=null? baseRule.ToPascalcase():"";

          
            SyntaxKind          = $"{rule.Name.ToPascalcase()}Syntax";

            int index=0;
        
                foreach (var element in rule.Alternatives.Single().Elements) {

                    var slotModel=new SlotMemberModel
                    {
                        Name= element.Name.ToPascalcase(),
                        IsToken= element is TokenElement,
                        SlotIndex=index++
                    };
                    Slots.Add(slotModel);

                // TODO ab hier eigentlich obsolet...
                    if (element is TokenElement) {

                        TokenSlots.Add(new SlotMemberModel
                        {
                            Name = $"{element.Name.ToPascalcase()}Token",
                            Type = "TokenSlot",
                        });

                        SyntaxTokens.Add(new SlotMemberModel
                        {
                            Name = $"{element.Name.ToPascalcase()}Token",
                            Type = "SyntaxToken",
                        });
                    }

                    if (element is RuleElement ruleElement) {

                        var baseName   = $"{ruleElement.Name.ToPascalcase()}Syntax";
                        var slotName   = $"{baseName}Slot";
                        var syntaxName = $"{baseName}";

                        SyntaxSlots.Add(new SlotMemberModel
                        {
                            Name = $"{ruleElement.Name.ToPascalcase()}Syntax",
                            Type = slotName,
                        });

                        SyntaxNodes.Add(new SlotMemberModel
                        {
                            Name = $"{ruleElement.Name.ToPascalcase()}",
                            Type = syntaxName,
                        });
                    }
            }

        }

    }

}