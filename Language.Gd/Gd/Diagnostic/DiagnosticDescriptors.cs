using System;

namespace Pharmatechnik.Language.Gd {

    public static class DiagnosticDescriptors {

        public static DiagnosticDescriptor NewInternalError(Exception ex) {
            return new DiagnosticDescriptor(
                id             : DiagnosticId.Gd0001,
                messageFormat  : "Internal error: " + ex.Message,
                category       : DiagnosticCategory.Internal,
                defaultSeverity: DiagnosticSeverity.Error
            );
        }

        public static DiagnosticDescriptor NewSyntaxError(string errorMessage) {
            return new DiagnosticDescriptor(
                id             : DiagnosticId.Gd0002,
                messageFormat  : errorMessage,
                category       : DiagnosticCategory.Syntax,
                defaultSeverity: DiagnosticSeverity.Error
            );
        }       
    }

}