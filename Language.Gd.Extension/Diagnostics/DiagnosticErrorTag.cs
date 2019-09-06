#region Using Directives

using Microsoft.VisualStudio.Text.Tagging;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Diagnostics {

    public sealed class DiagnosticErrorTag: IErrorTag {

        readonly Diagnostic _diagnostic;

        public DiagnosticErrorTag(Diagnostic diagnostic) {
            _diagnostic = diagnostic;
        }

        public string ErrorType {
            get {
                // Sonderlocke "Dead Code"
                if (_diagnostic.Category == DiagnosticCategory.DeadCode &&
                    _diagnostic.Severity != DiagnosticSeverity.Error) {
                    // "Suggestion" führt zu einem unsichbaren squiggle,
                    // für den aber dennoch ein Tooltip angezeigt wird.
                    // "Dead Code" wird durch leichtes Ausblenden extra visualisiert.
                    return DiagnosticErrorTypeNames.Suggestion;
                }

                switch (_diagnostic.Severity) {
                    case DiagnosticSeverity.Suggestion:
                        return DiagnosticErrorTypeNames.Suggestion;
                    case DiagnosticSeverity.Warning:
                        return DiagnosticErrorTypeNames.Warning;
                    case DiagnosticSeverity.Error:
                    default:
                        return DiagnosticErrorTypeNames.Error;
                }
            }
        }

        public object ToolTipContent {
            get { return _diagnostic.Message; }
        }

        public Diagnostic Diagnostic {
            get { return _diagnostic; }
        }

    }

}