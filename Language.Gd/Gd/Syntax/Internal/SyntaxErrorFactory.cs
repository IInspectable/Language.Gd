using Antlr4.Runtime;

using JetBrains.Annotations;

using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Internal {

    static class SyntaxErrorFactory {

        public static Diagnostic CreateDiagnostic(SourceText sourceText, [CanBeNull] IToken offendingSymbol, int line, int charPositionInLine, string msg) {

            if (offendingSymbol != null) {
                var textExtent = TextExtent.FromBounds(start: offendingSymbol.StartIndex, end: offendingSymbol.StopIndex + 1);
                var location   = sourceText.GetLocation(textExtent);
                return CreateDiagnostic(msg, location);
            }

            return CreateDiagnostic(sourceText: sourceText, line: line, charPositionInLine: charPositionInLine, msg: msg);
        }

        public static Diagnostic CreateDiagnostic(SourceText sourceText, int line, int charPositionInLine, string msg) {

            var textLine = sourceText.TextLines[line - 1];
            var location = textLine.GetLocation(charPositionInLine, 1);

            return CreateDiagnostic(msg, location);
        }

        static Diagnostic CreateDiagnostic(string msg, Location location) {

            return new Diagnostic(location, DiagnosticDescriptors.NewSyntaxError(msg));
        }

    }

}