using System.Collections.Immutable;
using System.Threading;

using Pharmatechnik.Language.Gd.Antlr;
using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd {

    public class SyntaxTree {

        readonly SyntaxSlot _rootSlot;

        SyntaxTree(SourceText sourceText, 
                   SyntaxSlot rootSlot,
                   ImmutableArray<TokenSlot> tokens, 
                   ImmutableArray<Diagnostic> diagnostics) {
            SourceText  = sourceText;
            Tokens      = tokens;
            Diagnostics = diagnostics;
            _rootSlot = rootSlot;

        }

        SyntaxNode _root;
        public SyntaxNode Root {
            get {
                var root = _root;
                if (root == null) {
                    Interlocked.CompareExchange(ref _root, _rootSlot.Realize(this, null), null);
                    root = _root;
                }

                return root;
            }
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
            var visitor = new GdSyntaxSlotBuilder(tokens);
            var slot=visitor.Visit(tree);

            //slot.Realize(this, null);
            // SyntaxTree, etc

            return new SyntaxTree(sourceText, slot, tokens, diagnostics.ToImmutableArray());
        }

    }

}