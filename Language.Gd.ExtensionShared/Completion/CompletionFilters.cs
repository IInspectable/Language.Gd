#region Using Directives

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Completion {

    static class CompletionFilters {

        public static readonly CompletionFilter Keywords = new CompletionFilter("Keywords", "K", CompletionImages.Keyword);

    }

}