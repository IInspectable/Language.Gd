using System;
using System.IO;
using System.Text;

using Antlr4.Runtime;

using Tool.GdSyntaxGenerator.Grammar;
using Tool.GdSyntaxGenerator.Models;

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
            foreach (var token in tokenInfo.Tokens) {
                Console.WriteLine($"{token.Index}:{token.Name}");
            }

            var grammarInfo = ReadGrammarInfo(Resources.GdGrammar);
            foreach (var rule in grammarInfo.Rules) {
                Console.WriteLine();
                Console.WriteLine(rule);
            }

            WriteGeneratedFiles(targetDirectory, baseNamespace, tokenInfo, grammarInfo);
        }

        static void WriteGeneratedFiles(string targetDirectory, string baseNamespace, TokenInfo tokenInfo, GrammarInfo grammarInfo) {

            var syntaxKindModel = new SyntaxKindEnumModel(
                @namespace: baseNamespace,
                tokenInfo: tokenInfo,
                grammarInfo: grammarInfo
            );
            var slotModels = new SlotModels(
                baseNamespace: baseNamespace,
                grammarInfo: grammarInfo
            );

            Console.WriteLine(targetDirectory);

            if (!Directory.Exists(targetDirectory)) {
                Directory.CreateDirectory(targetDirectory);
            }

            WriteSyntaxKind(targetDirectory, syntaxKindModel);
            WriteSyntaxSlots(targetDirectory, slotModels);
            WriteSyntaxNodes(targetDirectory, slotModels);
            WriteSyntaxSlotBuilder(targetDirectory, slotModels);
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
                                            SyntaxKindEnumModel model) {

            var context  = new CodeGeneratorContext();
            var content  = CodeGenerator.GenerateSyntaxKindEnum(model, context);
            var fullname = Path.Combine(targetDirectory, "SyntaxKind.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

        private static void WriteSyntaxSlots(string targetDirectory,
                                             SlotModels model) {

            var context  = new CodeGeneratorContext();
            var content  = CodeGenerator.GenerateSyntaxSlot(model, context);
            var fullname = Path.Combine(targetDirectory, "SyntaxSlot.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

        private static void WriteSyntaxNodes(string targetDirectory,
                                             SlotModels model) {

            var context  = new CodeGeneratorContext();
            var content  = CodeGenerator.GenerateSyntaxNode(model, context);
            var fullname = Path.Combine(targetDirectory, "Syntax.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

        private static void WriteSyntaxSlotBuilder(string targetDirectory,
                                                   SlotModels model) {

            var context  = new CodeGeneratorContext();
            var content  = CodeGenerator.GenerateSyntaxSlotBuilder(model, context);
            var fullname = Path.Combine(targetDirectory, "SyntaxSlotBuilder.generated.cs");

            File.WriteAllText(fullname, content, Encoding.UTF8);
        }

    }

}