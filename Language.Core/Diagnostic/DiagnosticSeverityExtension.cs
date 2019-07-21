using System.Collections.Generic;

namespace Pharmatechnik.Language {

    public static class DiagnosticSeverityExtension {

        public static DiagnosticSeverity? GetWorst(this DiagnosticSeverity value1, DiagnosticSeverity value2) {
            return (int) value2 > (int) value1 ? value2 : value1;
        }

        public static DiagnosticSeverity? GetWorst(this DiagnosticSeverity? value1, DiagnosticSeverity? value2) {
            if (value1 == null) {
                return value2;
            }

            if (value2 == null) {
                return value1;
            }

            return GetWorst(value1.Value, value2.Value);
        }

        public static DiagnosticSeverity? GetWorst(this IEnumerable<DiagnosticSeverity> values) {
            DiagnosticSeverity? worst = null;
            foreach (var v in values) {
                // Shortcut: Error
                if (v == DiagnosticSeverity.Error) {
                    return DiagnosticSeverity.Error;
                }

                worst = worst.GetWorst(v);
            }

            return worst;
        }

    }

}