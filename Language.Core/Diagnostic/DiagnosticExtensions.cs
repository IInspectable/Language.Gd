#region Using Directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Pharmatechnik.Language {

    public static class DiagnosticExtensions {

        public static bool HasErrors(this IEnumerable<Diagnostic> source) {
            return source.Errors().Any();
        }

        public static IEnumerable<Diagnostic> Warnings(this IEnumerable<Diagnostic> source) {
            return source.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Warning);
        }

        public static IEnumerable<Diagnostic> Errors(this IEnumerable<Diagnostic> source) {
            return source.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error);
        }

        public static IEnumerable<Diagnostic> Suggestions(this IEnumerable<Diagnostic> source) {
            return source.Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Suggestion);
        }

    }

}