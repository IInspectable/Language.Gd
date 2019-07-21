using System;
using System.IO;
using System.Text;

using Antlr4.Runtime;

using Tool.GdSyntaxGenerator.Grammar;
using Tool.GdSyntaxGenerator.Models;

namespace Tool.GdSyntaxGenerator {

    class Program {

        public static void Main(string[] args) {

            // TODO Pfade als Parameter
            const string grammarPath = @"C:\ws\git\Language.Gd\Language.Gd\Gd\Grammar\GdGrammar.g4";
            const string tokenPath   = @"C:\ws\git\Language.Gd\Language.Gd\Gd\Grammar\GdTokens.g4";

            var tokenInfo = ReadTokenInfo(tokenPath);
            foreach (var token in tokenInfo.Tokens) {
                Console.WriteLine($"{token.Index}:{token.Name}");
            }

            var grammarInfo = ReadGrammarInfo(grammarPath);
            foreach (var rule in grammarInfo.Rules) {
                Console.WriteLine(rule);
            }

            WriteModel(args, tokenInfo, grammarInfo);
        }

        static void WriteModel(string[] args, TokenInfo tokenInfo, GrammarInfo grammarInfo) {

            var targetNamespace = args[0];
            var targetDirectory = args[1];

            Console.WriteLine(targetNamespace);
            Console.WriteLine(targetDirectory);

            if (!Directory.Exists(targetDirectory)) {
                Directory.CreateDirectory(targetDirectory);
            }

            var syntaxKindModel = new SyntaxKindEnumModel(
                tokenInfo: tokenInfo,
                grammarInfo: grammarInfo
            );

            WriteSyntaxKind(targetDirectory, syntaxKindModel);

            var slotModels = new SlotModels(
                grammarInfo: grammarInfo
            );

            WriteSyntaxSlots(targetDirectory, slotModels);
            WriteSyntaxNodes(targetDirectory, slotModels);
        }

        static GrammarInfo ReadGrammarInfo(string grammarPath) {

            string input = File.ReadAllText(grammarPath);

            ICharStream stream = new AntlrInputStream(input);
            var         lexer  = new AntlrV4Tokens(stream);

            // Setup Parser
            var cts    = new CommonTokenStream(lexer);
            var parser = new AntlrV4Grammar(cts);

            var tree = parser.grammarSpec();

            var grammarInfo = new GrammarInfo(tree);

            return grammarInfo;

        }

        static TokenInfo ReadTokenInfo(string tokenPath) {

            var input = File.ReadAllText(tokenPath);

            ICharStream stream = new AntlrInputStream(input);
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

    }

}