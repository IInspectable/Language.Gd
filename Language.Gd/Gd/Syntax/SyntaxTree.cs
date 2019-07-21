using System.Collections.Immutable;

using Pharmatechnik.Language.Gd.Antlr;
using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd {

    public class SyntaxTree {

        SyntaxTree(SourceText sourceText, ImmutableArray<TokenSlot> tokens, ImmutableArray<Diagnostic> diagnostics) {
            SourceText  = sourceText;
            Tokens      = tokens;
            Diagnostics = diagnostics;

        }

        public   SourceText                 SourceText  { get; }
        internal ImmutableArray<TokenSlot>  Tokens      { get; }
        public   ImmutableArray<Diagnostic> Diagnostics { get; }

        public static SyntaxTree Parse(SourceText sourceText) {

            var diagnostics = ImmutableArray.CreateBuilder<Diagnostic>();

            // Setup Lexer
            var stream             = sourceText.ToCharStream();
            var lexer              = new GdTokens(stream);
            var lexerErrorListener = new GdLexerErrorListener(sourceText, diagnostics);
            lexer.RemoveErrorListeners();
            lexer.AddErrorListener(lexerErrorListener);

            // Setup Parser
            var cts                 = new GdCommonTokenStream(lexer);
            var parser              = new GdGrammar(cts);
            var parserErrorListener = new GdParserErrorListener(sourceText, diagnostics);
            parser.RemoveErrorListeners();
            parser.AddErrorListener(parserErrorListener);

            var tree = parser.guiDescription();

            var errorNodes = new ErrorNodeVisitor();
            errorNodes.Visit(tree);

            var tokens  = TokenFactory.CreateTokens(cts, errorNodes.ErrorNodes);
            var visitor = new GdGrammarVisitor(tokens);
            visitor.Visit(tree);

            // SyntaxTree, etc

            return new SyntaxTree(sourceText, tokens, diagnostics.ToImmutableArray());
        }

    }

}