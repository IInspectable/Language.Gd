#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

#endregion

namespace Tool.GdSyntaxGenerator.Models {

    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    class SlotModel: AbstractSlotModel {

        public SlotModel(ParserRule rule, string baseRule, string baseNamespace, GrammarInfo grammarInfo)
            : base(rule: rule, baseNamespace: baseNamespace) {

            if (rule.Alternatives.Count != 1) {
                throw new ArgumentException();
            }

            BaseRuleName = baseRule?.ToPascalcase();
            SyntaxKind   = $"{rule.Name.ToPascalcase()}Syntax";

            int index = 0;

            if (IsSection) {

                // Überprüfen, ob es sich um eine benannte Sektion handelt
                var expectedSectionName = RuleName.Replace("Section", "Identifier");

                // Die Begin-/End Sektionen muss es geben, andernfalls ist die Grammatik falsch formuliert
                var sb     = grammarInfo.Rules.Single(r => r.Name?.ToPascalcase()                        == SectionBeginName);
                var sbName = sb.Alternatives.Single().Elements.FirstOrDefault(e => e.Name.ToPascalcase() == expectedSectionName);

                var se     = grammarInfo.Rules.Single(r => r.Name?.ToPascalcase()                        == SectionEndName);
                var seName = se.Alternatives.Single().Elements.FirstOrDefault(e => e.Name.ToPascalcase() == expectedSectionName);

                if (sbName != null && seName != null) {
                    IsNamedSection             = true;
                    NamedSectionIdentifierName = expectedSectionName;
                }
            }

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
                    Name        = name,
                    IsToken     = element is TokenElement,
                    IsLabeled   = element.IsLabeled,
                    SyntaxKind  = syntaxKind,
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

        public bool IsSection => RuleName.EndsWith("Section");

        public string SectionBeginName => IsSection ? RuleName + "Begin" : "";
        public string SectionEndName   => IsSection ? RuleName + "End" : "";

        public bool   IsNamedSection             { get; }
        public string NamedSectionIdentifierName { get; }

    }

}