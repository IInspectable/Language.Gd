#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.QuickInfo {

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name(QuickInfoSourceProviderNames.DebugQuickInfoSourceProvider)]
    [Order(After = QuickInfoSourceProviderNames.QuickInfoSourceProvider)]
    [ContentType(GdLanguageContentDefinitions.ContentType)]
    class DebugQuickInfoSourceProvider: IAsyncQuickInfoSourceProvider {

        [ImportingConstructor]
        public DebugQuickInfoSourceProvider() {
        }

        IAsyncQuickInfoSource IAsyncQuickInfoSourceProvider.TryCreateQuickInfoSource(ITextBuffer textBuffer) {
            return new DebugQuickInfoSource(textBuffer);
        }

    }

}