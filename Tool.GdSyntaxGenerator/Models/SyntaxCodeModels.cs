using System;
using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Language.Text;

namespace Tool.GdSyntaxGenerator.Models {

    class SyntaxCodeModels {

        public SyntaxCodeModels(string @namespace,
                                TokenInfo tokenInfo,
                                GrammarInfo grammarInfo) {

            SlotNamespace   = $"{@namespace}.Internal";
            SyntaxNamespace = @namespace;

            Dictionary<string, string> baseRules = new Dictionary<string, string>();
            foreach (var parserRule in grammarInfo.Rules.Where(rule => rule.Alternatives.Count > 1)) {
                AbstractClasses.Add(new AbstractSyntaxModel(parserRule));
                foreach (var alternative in parserRule.Alternatives) {
                    baseRules[alternative.Elements[0].Name] = parserRule.Name;
                }
            }

            foreach (var parserRule in grammarInfo.Rules.Where(rule => rule.Alternatives.Count == 1)) {
                baseRules.TryGetValue(parserRule.Name, out var baseRule);
                Classes.Add(new SyntaxModel(parserRule, baseRule));
            }

        }

        public string SlotNamespace   { get; }
        public string SyntaxNamespace { get; }

        public List<SyntaxModel>         Classes         { get; } = new List<SyntaxModel>();
        public List<AbstractSyntaxModel> AbstractClasses { get; } = new List<AbstractSyntaxModel>();

    }

    class PropertyModel {

        public string Type          { get; set; }
        public string Name          { get; set; }
        public string ParameterName => Name.ToCamelcase();
        public string FieldName     => $"_{Name.ToCamelcase()}";

    }

    class AbstractSyntaxModel {

        public AbstractSyntaxModel(ParserRule rule) {
            // TODO richtige Basisklasse?
            SyntaxBaseClassName = "SyntaxNode";
            SyntaxClassName     = $"{rule.Name.ToPascalcase()}Syntax";
            SlotClassName       = $"{rule.Name.ToPascalcase()}SyntaxSlot";
        }

        public string SyntaxBaseClassName { get; }
        public string SyntaxClassName     { get; }
        public string SlotClassName       { get; }

    }

    class SyntaxModel {

        public string SyntaxBaseClassName { get; }
        public string SyntaxClassName     { get; }
        public string SlotClassName       { get; }
        public string SyntaxKind          { get; }

        public List<PropertyModel> TokenSlots  { get; } = new List<PropertyModel>();
        public List<PropertyModel> SyntaxSlots { get; } = new List<PropertyModel>();

        public List<PropertyModel> SyntaxTokens { get; } = new List<PropertyModel>();
        public List<PropertyModel> SyntaxNodes  { get; } = new List<PropertyModel>();

        public SyntaxModel(ParserRule rule, string baseRule) {

            if (rule.Alternatives.Count != 1) {
                throw new ArgumentException();
            }

            // TODO richtige Basisklasse
            SyntaxBaseClassName = baseRule != null ? $"{baseRule.ToPascalcase()}Syntax" : $"SyntaxNode";
            SyntaxClassName     = $"{rule.Name.ToPascalcase()}Syntax";
            SlotClassName       = $"{rule.Name.ToPascalcase()}SyntaxSlot";
            SyntaxKind          = $"{rule.Name.ToPascalcase()}Syntax";

            foreach (var alternative in rule.Alternatives) {
                foreach (var element in alternative.Elements) {

                    if (element is TokenElement) {
                        TokenSlots.Add(new PropertyModel {
                            Name = $"{element.Name.ToPascalcase()}Token",
                            Type = "TokenSlot",
                        });

                        SyntaxTokens.Add(new PropertyModel {
                            Name = $"{element.Name.ToPascalcase()}Token",
                            Type = "SyntaxToken",
                        });
                    }

                    if (element is RuleElement ruleElement) {

                        var baseName   = $"{ruleElement.Name.ToPascalcase()}Syntax";
                        var slotName   = $"{baseName}Slot";
                        var syntaxName = $"{baseName}";

                        SyntaxSlots.Add(new PropertyModel {
                            Name = $"{ruleElement.Name.ToPascalcase()}Syntax",
                            Type = slotName,
                        });

                        SyntaxNodes.Add(new PropertyModel {
                            Name = $"{ruleElement.Name.ToPascalcase()}",
                            Type = syntaxName,
                        });
                    }
                }
            }

        }

    }

}