#region Using Directives

using System.Threading;
using System.Collections.Immutable;

using Pharmatechnik.Language.Gd.Antlr;
using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd {

    public class SyntaxTree {

        readonly SyntaxSlot _rootSlot;

        SyntaxTree(SourceText sourceText,
                   SyntaxSlot rootSlot,
                   ImmutableArray<Diagnostic> diagnostics) {
            SourceText  = sourceText;
            Diagnostics = diagnostics;
            _rootSlot   = rootSlot;

        }

        SyntaxNode _rootNode;

        public SyntaxNode Root {
            get {
                var root = _rootNode;
                if (root == null) {
                    Interlocked.CompareExchange(ref _rootNode, _rootSlot.Realize(this, null, position: 0), null);
                    root = _rootNode;
                }

                return root;
            }
        }

        public SourceText                 SourceText  { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }

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
            var slot    = visitor.Visit(tree);

            return new SyntaxTree(sourceText, slot, diagnostics.ToImmutableArray());
        }

    }

}