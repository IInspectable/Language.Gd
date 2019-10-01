#region Using Directives

using System.ComponentModel.Composition;
using System.Windows;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Margin {

    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(GdMargin.MarginName)]
    [Order(After  = PredefinedMarginNames.ZoomControl)]
    [Order(Before = PredefinedMarginNames.FileHealthIndicator)]
    [Order(Before = PredefinedMarginNames.HorizontalScrollBarContainer)]
    [MarginContainer(PredefinedMarginNames.BottomControl)]
    [ContentType(GdLanguageContentDefinitions.ContentType)]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    [GridUnitType(GridUnitType.Auto)]
    [GridCellLength(1.0)]
    sealed class GdMarginProvider: IWpfTextViewMarginProvider {

        /// <summary>
        /// Creates an <see cref="IWpfTextViewMargin"/> for the given <see cref="IWpfTextViewHost"/>.
        /// </summary>
        /// <param name="wpfTextViewHost">The <see cref="IWpfTextViewHost"/> for which to create the <see cref="IWpfTextViewMargin"/>.</param>
        /// <param name="marginContainer">The margin that will contain the newly-created margin.</param>
        /// <returns>The <see cref="IWpfTextViewMargin"/>.
        /// The value may be null if this <see cref="IWpfTextViewMarginProvider"/> does not participate for this context.
        /// </returns>
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost wpfTextViewHost, IWpfTextViewMargin marginContainer) {
            return new GdMargin(wpfTextViewHost.TextView);
        }

    }

}