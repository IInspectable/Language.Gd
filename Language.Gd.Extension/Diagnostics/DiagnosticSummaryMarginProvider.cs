#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Diagnostics {

    [Export(typeof(IWpfTextViewMarginProvider))]
    [Order(After  = "SplitterControl")]
    [Order(Before = PredefinedMarginNames.VerticalScrollBarContainer)]
    [MarginContainer(PredefinedMarginNames.RightControl)]
    [Name(DiagnosticSummaryMargin.MarginName)]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    [ContentType(GdLanguageContentDefinitions.ContentType)]
    class DiagnosticSummaryMarginProvider: IWpfTextViewMarginProvider {

        /// <summary>
        /// Creates an <see cref="IWpfTextViewMargin"/> for the given <see cref="IWpfTextViewHost"/>.
        /// </summary>
        /// <param name="wpfTextViewHost">The <see cref="IWpfTextViewHost"/> for which to create the <see cref="IWpfTextViewMargin"/>.</param>
        /// <param name="marginContainer">The margin that will contain the newly-created margin.</param>
        /// <returns>The <see cref="IWpfTextViewMargin"/>.
        /// The value may be null if this <see cref="IWpfTextViewMarginProvider"/> does not participate for this context.
        /// </returns>
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer) {
            return new DiagnosticSummaryMargin(wpfTextViewHost.TextView);
        }

    }

}