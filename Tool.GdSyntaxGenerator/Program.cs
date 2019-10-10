//#define Verbose

#region Using Directives

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Antlr4.Runtime;

using JetBrains.Annotations;

using Pharmatechnik.Apotheke.XTplus.Framework.Tools.Generators.GuiModelGenerator;
using Pharmatechnik.Apotheke.XTplus.Framework.Tools.Generators.GuiModelGenerator.GuiMetaModel;

using Tool.GdSyntaxGenerator.Grammar;
using Tool.GdSyntaxGenerator.Models;

#endregion

namespace Tool.GdSyntaxGenerator {

    class Program {

        public static void Main(string[] args) {

            var targetDirectory = Path.GetTempPath();
            if (args.Length > 0) {
                targetDirectory = args[0];
            }

            var baseNamespace = "Unspecified";
            if (args.Length > 1) {
                baseNamespace = args[1];
            }

            var tokenInfo = ReadTokenInfo(Resources.GdTokens);
            DumpTokens(tokenInfo);

            var grammarInfo = ReadGrammarInfo(Resources.GdGrammar);
            DumpGrammar(grammarInfo);

            WriteGeneratedFiles(targetDirectory, baseNamespace, tokenInfo, grammarInfo);
        }

        [Conditional("Verbose")]
        private static void DumpTokens(TokenInfo tokenInfo) {

            foreach (var token in tokenInfo.Tokens) {
                if (token.IsSimpleTerminal) {
                    WriteVerbose($"{token.Index}:{token.Name} = {token.TerminalText}");
                } else {
                    WriteVerbose($"{token.Index}:{token.Name}");
                }

            }
        }

        [Conditional("Verbose")]
        private static void DumpGrammar(GrammarInfo grammarInfo) {

            foreach (var rule in grammarInfo.Rules) {
                WriteVerbose();
                WriteVerbose(rule);
            }
        }

        [Conditional("Verbose")]
        static void WriteVerbose(object value) {
            Console.WriteLine(value);
        }

        [Conditional("Verbose")]
        static void WriteVerbose() {
            Console.WriteLine();
        }

        static void WriteGeneratedFiles(string targetDirectory,
                                        string baseNamespace,
                                        TokenInfo tokenInfo,
                                        GrammarInfo grammarInfo) {

            var syntaxKindModel = new SyntaxKindEnumModel(
                @namespace: baseNamespace,
                tokenInfo: tokenInfo,
                grammarInfo: grammarInfo
            );
            var slotModels = new SlotModels(
                baseNamespace: baseNamespace,
                grammarInfo: grammarInfo
            );

            var tokenModel = new TokenModel(
                tokenInfo: tokenInfo,
                baseNamespace: baseNamespace);

            Console.WriteLine(targetDirectory);

            if (!Directory.Exists(targetDirectory)) {
                Directory.CreateDirectory(targetDirectory);
            }

            var context = new CodeGeneratorContext();

            WriteSyntaxKind(targetDirectory, syntaxKindModel, context);
            WriteSyntaxSlots(targetDirectory, slotModels, context);
            WriteSyntaxNodes(targetDirectory, slotModels, context);
            WriteSyntaxSlotBuilder(targetDirectory, slotModels, context);
            WriteSyntaxVisitor(targetDirectory, slotModels, context);
            WriteSyntaxFactory(targetDirectory, slotModels, context);
            WriteSyntaxFacts(targetDirectory, tokenModel, context);
            WriteMetaModel(targetDirectory, slotModels, context);

        }

        static GrammarInfo ReadGrammarInfo(string grammarSpec) {

            ICharStream stream = new AntlrInputStream(grammarSpec);
            var         lexer  = new AntlrV4Tokens(stream);

            // Setup Parser
            var cts    = new CommonTokenStream(lexer);
            var parser = new AntlrV4Grammar(cts);

            var tree = parser.grammarSpec();

            var grammarInfo = new GrammarInfo(tree);

            return grammarInfo;

        }

        static TokenInfo ReadTokenInfo(string tokenSpec) {

            ICharStream stream = new AntlrInputStream(tokenSpec);
            var         lexer  = new AntlrV4Tokens(stream);

            // Setup Parser
            var cts    = new CommonTokenStream(lexer);
            var parser = new AntlrV4Grammar(cts);

            var tree = parser.grammarSpec();

            var tokenInfo = new TokenInfo(tree);

            return tokenInfo;
        }

        private static void WriteSyntaxKind(string targetDirectory,
                                            SyntaxKindEnumModel model,
                                            CodeGeneratorContext context) {

            var content  = CodeGenerator.GenerateSyntaxKindEnum(model, context);
            var fullname = Path.Combine(targetDirectory, "SyntaxKind.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

        private static void WriteSyntaxSlots(string targetDirectory,
                                             SlotModels model,
                                             CodeGeneratorContext context) {

            var content  = CodeGenerator.GenerateSyntaxSlot(model, context);
            var fullname = Path.Combine(targetDirectory, "SyntaxSlot.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

        private static void WriteSyntaxNodes(string targetDirectory,
                                             SlotModels model,
                                             CodeGeneratorContext context) {

            var content  = CodeGenerator.GenerateSyntaxNode(model, context);
            var fullname = Path.Combine(targetDirectory, "Syntax.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

        private static void WriteSyntaxSlotBuilder(string targetDirectory,
                                                   SlotModels model,
                                                   CodeGeneratorContext context) {

            var content  = CodeGenerator.GenerateSyntaxSlotBuilder(model, context);
            var fullname = Path.Combine(targetDirectory, "SyntaxSlotBuilder.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

        private static void WriteSyntaxVisitor(string targetDirectory,
                                               SlotModels model,
                                               CodeGeneratorContext context) {

            var content  = CodeGenerator.GenerateSyntaxVisitor(model, context);
            var fullname = Path.Combine(targetDirectory, "SyntaxVisitor.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

        private static void WriteSyntaxFactory(string targetDirectory,
                                               SlotModels model,
                                               CodeGeneratorContext context) {

            var content  = CodeGenerator.GenerateSyntaxFactory(model, context);
            var fullname = Path.Combine(targetDirectory, "SyntaxFactory.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

        private static void WriteSyntaxFacts(string targetDirectory, TokenModel model, CodeGeneratorContext context) {
            var content  = CodeGenerator.GenerateSyntaxFacts(model, context);
            var fullname = Path.Combine(targetDirectory, "SyntaxFacts.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

        [UsedImplicitly]
        private static void WriteMetaModel(string targetDirectory, SlotModels slotModels, CodeGeneratorContext context) {

            // TODO WriteMetaModel

            var guiElements = from t in typeof(GuiElement).Assembly.GetTypes()
                              where t.IsSubclassOf(typeof(GuiElement))
                              select t;

            foreach (var c in guiElements) {
                Console.WriteLine($"{c.Name} {c.BaseType?.Name}");
                //WriteVerbose(c.Name);
            }

            var controls = from t in typeof(GuiElement).Assembly.GetTypes()
                           where t.IsSubclassOf(typeof(Control))
                           select t;

            var controlTypesToSkip = new[] {
                (Type: typeof(EditorUIControl), SkipDerivates: true),
                (Type: typeof(FunctionButtonBar), SkipDerivates: true),
                (Type: typeof(FormFunctionBar), SkipDerivates: true),
                (Type: typeof(CustomControl), SkipDerivates: true),
                (Type: typeof(SelectionList), SkipDerivates: true),
                (Type: typeof(RadiobuttonGroup), SkipDerivates: true),
                (Type: typeof(TableWithPaging), SkipDerivates: true),
                (Type: typeof(ExtendedControl), SkipDerivates: true),
                (Type: typeof(TabNavigation), SkipDerivates: true),
                (Type: typeof(TabPage), SkipDerivates: true),
                (Type: typeof(TabStrip), SkipDerivates: true),
                (Type: typeof(Listbox), SkipDerivates: true),

            }.ToDictionary(i => i.Type);

            Console.WriteLine("--------------------");

            var unknownProperties = new List<(string ControlName, string PropertyName)>();

            foreach (var c in controls.OrderBy(c => c.Name)) {

                if (controlTypesToSkip.ContainsKey(c)) {
                    continue;
                }

                if (c.BaseType != null && controlTypesToSkip.TryGetValue(c.BaseType, out var skipInfo) && skipInfo.SkipDerivates) {
                    continue;
                }

                var s = (SupportedPropertiesAttribute[]) Attribute.GetCustomAttributes(c, typeof(SupportedPropertiesAttribute), true);

                var props = s.Where(sp => sp.GetType() == typeof(SupportedPropertiesAttribute))
                             .SelectMany(spa => spa.Properties)
                             .Select(p => new {
                                  Name     = p.Name,
                                  Reqired  = p.Required,
                                  Property = c.GetProperty(p.Name)
                              })
                             .ToList();

                Console.WriteLine($"{c.Name},");
                foreach (var prop in props) {

                    if (prop.Property == null) {
                        unknownProperties.Add((ControlName: c.Name, PropertyName: prop.Name));
                        continue;
                    }

                    Console.WriteLine($"    {(prop.Reqired ? "*" : "")}{prop.Name}: {prop.Property.PropertyType.Name}");

                }

                //Console.WriteLine($"{c.Name} {c.BaseType?.Name}");
                //WriteVerbose(c.Name);
            }

            foreach (var up in unknownProperties) {
                Console.WriteLine($"## {up.ControlName}.{up.PropertyName}");
            }
        }

    }

}