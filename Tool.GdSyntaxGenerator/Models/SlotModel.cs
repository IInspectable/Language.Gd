using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models {

    class SlotModel: AbstractSlotModel {

        public SlotModel(ParserRule rule, string baseRule, string baseNamespace): base(rule: rule, baseNamespace: baseNamespace) {

            if (rule.Alternatives.Count != 1) {
                throw new ArgumentException();
            }

            BaseRuleName = baseRule?.ToPascalcase();
            SyntaxKind   = $"{rule.Name.ToPascalcase()}Syntax";

            int index = 0;

            foreach (var element in rule.Alternatives.Single().Elements) {

                var name = element.Name.ToPascalcase();
                if (name == "EOF") {
                    name = "Eof";
                }

                var syntaxKind = element.SyntaxKind;
                if (syntaxKind == "EOF") {
                    syntaxKind = "Eof";
                }

                Slots.Add(new SlotMemberModel {
                    Name       = name,
                    IsToken    = element is TokenElement,
                    IsLabeled  = element.IsLabeled,
                    SyntaxKind = syntaxKind,
                    // TODO Falls token, dann wollen wir hier immer eine 0..1 
                    Cardinality = element.Cardinality,
                    SlotIndex   = index++
                });
            }
        }

        public string BaseRuleName { get; }
        public string SyntaxKind   { get; }

        public List<SlotMemberModel>        Slots       { get; } = new List<SlotMemberModel>();
        public IEnumerable<SlotMemberModel> SyntaxSlots => Slots.Where(s => !s.IsToken);
        public IEnumerable<SlotMemberModel> TokenSlots  => Slots.Where(s => s.IsToken);

        // TODO Named Sections generieren?
        public bool   IsSection        => RuleName.EndsWith("Section");
        public string SectionBeginName => IsSection ? RuleName + "Begin" : "";
        public string SectionEndName   => IsSection ? RuleName + "End" : "";

    }

}