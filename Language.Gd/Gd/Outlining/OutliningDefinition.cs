using Pharmatechnik.Language.Text;

namespace Pharmatechnik.Language.Gd.Outlining {

    public class OutliningDefinition {

        public OutliningDefinition(string collapsedText,
                                   TextExtent foldingExtent,
                                   TextExtent previewExtent,
                                   bool isDefaultCollapsed,
                                   bool isImplementation) {

            CollapsedText      = collapsedText;
            FoldingExtent      = foldingExtent;
            PreviewExtent      = previewExtent;
            IsImplementation   = isImplementation;
            IsDefaultCollapsed = isDefaultCollapsed;

        }

        public string     CollapsedText      { get; }
        public TextExtent FoldingExtent      { get; }
        public TextExtent PreviewExtent      { get; }
        public bool       IsImplementation   { get; }
        public bool       IsDefaultCollapsed { get; }

    }

}