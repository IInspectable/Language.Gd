#region Using Directives

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Classification {

    // Zu Debugging Zwecken
    [Export(typeof(ITaggerProvider))]
    [ContentType(GdLanguageContentDefinitions.ContentType)]
    [TagType(typeof(IClassificationTag))]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    sealed class TriviaSpanClassificationTaggerProvider: ITaggerProvider {

        readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        [ImportingConstructor]
        public TriviaSpanClassificationTaggerProvider(IClassificationTypeRegistryService classificationTypeRegistryService) {
            _classificationTypeRegistryService = classificationTypeRegistryService;
        }

        public ITagger<T> CreateTagger<T>(ITextBuffer buffer) where T : ITag {

            return TriviaSpanClassificationTagger.GetOrCreateSingelton(_classificationTypeRegistryService, buffer) as ITagger<T>;

        }

    }

}