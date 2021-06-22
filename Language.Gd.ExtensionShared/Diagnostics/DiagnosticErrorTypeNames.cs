#region Using Directives

using Microsoft.VisualStudio.Text.Adornments;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Diagnostics {

    static class DiagnosticErrorTypeNames {

        public const string Error      = PredefinedErrorTypeNames.SyntaxError;
        public const string Warning    = PredefinedErrorTypeNames.Warning;
        public const string Suggestion = PredefinedErrorTypeNames.Suggestion;

    }

}