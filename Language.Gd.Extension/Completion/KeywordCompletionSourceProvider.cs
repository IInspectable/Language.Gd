#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Completion {

    [Export(typeof(IAsyncCompletionSourceProvider))]
    [ContentType(GdLanguageContentDefinitions.ContentType)]
    [Name(nameof(KeywordCompletionSourceProvider))]
    class KeywordCompletionSourceProvider: IAsyncCompletionSourceProvider {

        public IAsyncCompletionSource GetOrCreate(ITextView textView) {
            return new KeywordCompletionSource(textView);
        }

    }

}