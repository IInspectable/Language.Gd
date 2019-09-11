#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Language.Gd.Extension.Classification;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.QuickInfo {

    [Export(typeof(IAsyncQuickInfoSourceProvider))]
    [Name(QuickInfoSourceProviderNames.QuickInfoSourceProvider)]
    [Order(After = QuickInfoSourceProviderNames.DefaultQuickInfoPresenter)]
    [ContentType(GdLanguageContentDefinitions.ContentType)]
    class QuickInfoSourceProvider: IAsyncQuickInfoSourceProvider {

        readonly TextBlockBuilderService _textBlockBuilderService;

        [ImportingConstructor]
        public QuickInfoSourceProvider(TextBlockBuilderService textBlockBuilderService) {
            _textBlockBuilderService = textBlockBuilderService;
        }

        IAsyncQuickInfoSource IAsyncQuickInfoSourceProvider.TryCreateQuickInfoSource(ITextBuffer textBuffer) {
            return new QuickInfoSource(textBuffer, _textBlockBuilderService);
        }

    }

}