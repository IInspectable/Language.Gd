#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Completion {

    [Export(typeof(IAsyncCompletionCommitManagerProvider))]
    [Name(nameof(GdLanguageContentDefinitions.LanguageName) + nameof(CompletionCommitManagerProvider))]
    [ContentType(GdLanguageContentDefinitions.ContentType)]
    internal class CompletionCommitManagerProvider: IAsyncCompletionCommitManagerProvider {

        public IAsyncCompletionCommitManager GetOrCreate(ITextView textView) {
            return new CompletionCommitManager();
        }

    }

}