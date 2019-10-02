#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Diagnostics {

    [Export(typeof(ITaggerProvider))]
    [ContentType(GdLanguageContentDefinitions.ContentType)]
    [TagType(typeof(DiagnosticErrorTag))]
    sealed class DiagnosticErrorTaggerProvider: ITaggerProvider {

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            return new DiagnosticErrorTagger(buffer) as ITagger<T>;
        }

    }

}