using System;
using System.Collections.Immutable;

using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

using Pharmatechnik.Language.Gd.Internal;
using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Antlr {

    sealed class GdParserErrorListener: BaseErrorListener {

        public GdParserErrorListener(SourceText sourceText, ImmutableArray<Diagnostic>.Builder diagnostics) {
            SourceText  = sourceText  ?? throw new ArgumentNullException(nameof(sourceText));
            Diagnostics = diagnostics ?? throw new ArgumentNullException(nameof(diagnostics));
        }

        public SourceText                         SourceText  { get; }
        public ImmutableArray<Diagnostic>.Builder Diagnostics { get; }

        public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e) {
       
            var diagnostic = SyntaxErrorFactory.CreateDiagnostic(SourceText, line, charPositionInLine, msg);

            Diagnostics.Add(diagnostic);
        }

    }

}