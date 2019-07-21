using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models
{
    class SlotModel : AbstractSlotModel
    {

        public SlotModel(ParserRule rule, string baseRule) : base(rule)
        {

            if (rule.Alternatives.Count != 1)
            {
                throw new ArgumentException();
            }

            BaseRuleName = baseRule?.ToPascalcase();
            SyntaxKind = $"{rule.Name.ToPascalcase()}Syntax";

            int index = 0;

            foreach (var element in rule.Alternatives.Single().Elements)
            {

                Slots.Add(new SlotMemberModel
                {
                    Name = element.Name.ToPascalcase(),
                    IsToken = element is TokenElement,
                    // TODO Falls token, dann wollen wir hier immer eine 0..1 
                    Cardinality = element.Cardinality,
                    SlotIndex = index++
                });
            }
        }

        public string BaseRuleName { get; }
        public string SyntaxKind { get; }

        public List<SlotMemberModel> Slots { get; } = new List<SlotMemberModel>();
        public IEnumerable<SlotMemberModel> SyntaxSlots => Slots.Where(s => !s.IsToken);
        public IEnumerable<SlotMemberModel> TokenSlots => Slots.Where(s => s.IsToken);
                     
    }

}