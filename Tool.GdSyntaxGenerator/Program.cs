//#define Verbose

#region Using Directives

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

using Antlr4.Runtime;

using JetBrains.Annotations;

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
                    Console.WriteLine($"{token.Index}:{token.Name} = {token.TerminalText}");
                } else {
                    Console.WriteLine($"{token.Index}:{token.Name}");
                }

            }
        }

        [Conditional("Verbose")]
        private static void DumpGrammar(GrammarInfo grammarInfo) {

            foreach (var rule in grammarInfo.Rules) {
                Console.WriteLine();
                Console.WriteLine(rule);
            }
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
        private static void WriteMetaModel( string targetDirectory, SlotModels slotModels, CodeGeneratorContext context) {
            // TODO Write Control Types...

            var derivedTypes = from t in typeof(Control).Assembly.GetTypes()
                               where t.IsSubclassOf(typeof(GuiElement))
                               select t;

            foreach (var c in derivedTypes) {
                Console.WriteLine(c.Name);
            }
        }

    }

}