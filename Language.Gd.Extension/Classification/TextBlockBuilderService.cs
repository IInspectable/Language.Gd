#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text.Classification;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Classification {

    [Export]
    public sealed class TextBlockBuilderService {

        readonly IClassificationFormatMapService                            _classificationFormatMapService;
        readonly ImmutableDictionary<GdClassification, IClassificationType> _classificationMap;

        [ImportingConstructor]
        public TextBlockBuilderService(IClassificationFormatMapService classificationFormatMapService,
                                       IClassificationTypeRegistryService classificationTypeRegistryService) {

            _classificationFormatMapService = classificationFormatMapService;
            _classificationMap              = ClassificationTypeDefinitions.GetClassificationMap(classificationTypeRegistryService);

        }

        public IClassificationFormatMap ClassificationFormatMap => _classificationFormatMapService.GetClassificationFormatMap("tooltip");

        [CanBeNull]
        public TextBlock ToTextBlock(string text, GdClassification classification) {
            return ToTextBlock(new ClassifiedText(text, classification));
        }

        [CanBeNull]
        public TextBlock ToTextBlock(params ClassifiedText[] parts) {
            return ToTextBlock(parts.ToImmutableArray());
        }

        [CanBeNull]
        public TextBlock ToTextBlock(IReadOnlyCollection<ClassifiedText> parts) {

            if (parts.Count == 0) {
                return null;
            }

            var textBlock = new TextBlock {TextWrapping = TextWrapping.Wrap};

            textBlock.SetDefaultTextProperties(ClassificationFormatMap);

            foreach (var part in parts) {
                var inline = ToInline(part.Text, part.Classification, ClassificationFormatMap);
                textBlock.Inlines.Add(inline);
            }

            return textBlock;
        }

        Run ToInline(string text, GdClassification classification, IClassificationFormatMap formatMap) {

            var inline = new Run(text);

            _classificationMap.TryGetValue(classification, out var ct);
            if (ct != null) {
                var props = formatMap.GetTextProperties(ct);
                inline.SetTextProperties(props);
            }

            return inline;
        }

    }

}