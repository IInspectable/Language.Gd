using System.Collections.Generic;

using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models {

    class SyntaxKindEnumModel {
        
        public SyntaxKindEnumModel(string @namespace, TokenInfo tokenInfo, GrammarInfo grammarInfo) {
            
            Namespace = @namespace;

            EnumMembers.Add(
                new EnumMemberModel {
                    Name  = "None",
                    Value = 0
                });

            foreach (var token in tokenInfo.Tokens) {
                EnumMembers.Add(
                    new EnumMemberModel {
                        Name  = token.Name,
                        Value = token.Index
                    });
            }

            var syntaxEnumStart = 1000;

            foreach (var rule in grammarInfo.Rules) {
                EnumMembers.Add(
                    new EnumMemberModel {
                        Name  = $"{rule.Name.ToPascalcase()}Syntax",
                        Value = syntaxEnumStart++
                    });
            }

            EnumMembers.Add(
                new EnumMemberModel {
                    Name  = "SyntaxList",
                    Value = 2000
                });

        }

        public string Namespace { get; }

        public List<EnumMemberModel> EnumMembers { get; } = new List<EnumMemberModel>();

    }

}