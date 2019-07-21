using System.Collections.Generic;

using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models {

    class SyntaxKindEnumModel: CodeMemberModel {

        public SyntaxKindEnumModel(string @namespace, TokenInfo tokenInfo, GrammarInfo grammarInfo) {

            Namespace = @namespace;

            foreach (var token in tokenInfo.Tokens) {
                Members.Add(
                    new NamedValueModel {
                        Name  = token.Name,
                        Value = token.Index
                    });
            }

            var syntaxEnumStart = 1000;

            foreach (var rule in grammarInfo.Rules) {
                Members.Add(
                    new NamedValueModel {
                        Name  = $"{rule.Name.ToPascalcase()}Syntax",
                        Value = syntaxEnumStart++
                    });
            }

            Members.Add(
                new NamedValueModel {
                    Name  = "SyntaxList",
                    Value = 2000
                });

        }

        public List<NamedValueModel> Members { get; } = new List<NamedValueModel>();

    }

}