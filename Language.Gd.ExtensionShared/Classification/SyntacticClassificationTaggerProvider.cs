#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Classification {

    [ContentType(GdLanguageContentDefinitions.ContentType)]
    [Export(typeof(ITaggerProvider))]
    [TagType(typeof(IClassificationTag))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    sealed class SyntacticClassificationTaggerProvider: ITaggerProvider {

        readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        [ImportingConstructor]
        public SyntacticClassificationTaggerProvider(IClassificationTypeRegistryService classificationTypeRegistryService) {
            _classificationTypeRegistryService = classificationTypeRegistryService;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {
            return new SyntacticClassificationTagger(_classificationTypeRegistryService, buffer) as ITagger<T>;

        }

    }

}