#region Using Directives

using System.Linq;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.SmartIndent {

    class GdSmartIndent: ISmartIndent {

        private readonly ITextView _textView;

        public GdSmartIndent(ITextView textView) {
            _textView = textView;

        }

        public void Dispose() {

        }

        public int? GetDesiredIndentation(ITextSnapshotLine line) {

            var prevLine = GetPreviousNonEmptyLine(line);

            if (prevLine == null) {
                return 0;
            }

            var tabSize            = _textView.Options.GetTabSize();
            var desiredIndentation = prevLine.GetIndentationColumn(tabSize);

            if (IsLineSectionBegin(prevLine)) {
                desiredIndentation += tabSize;
            }

            return desiredIndentation;
        }

        bool IsLineSectionBegin(ITextSnapshotLine line) {

            var syntaxTree = TryGetSyntaxTree();

            var containingSection = syntaxTree?.Root
                                               .FindToken(line.End)
                                               .Parent?
                                               .Ancestors()
                                               .OfType<ISectionSyntax>()
                                               .FirstOrDefault();

            return containingSection?.SectionBegin?.Extent.IntersectsWith(line.End) == true;
        }

        [CanBeNull]
        SyntaxTree TryGetSyntaxTree() {

            var textBuffer = _textView.GetBufferContainingCaret();
            if (textBuffer == null) {
                return null;
            }

            var parserService = ParserService.ParserService.TryGet(textBuffer);
            return parserService?.SyntaxTreeAndSnapshot?.SyntaxTree;
        }

        [CanBeNull]
        static ITextSnapshotLine GetPreviousNonEmptyLine(ITextSnapshotLine line) {

            int lineNumber = line.LineNumber;

            while (lineNumber > 0) {

                lineNumber--;
                var prevLine = line.Snapshot.GetLineFromLineNumber(lineNumber);
                if (!string.IsNullOrWhiteSpace(prevLine.GetText())) {
                    return prevLine;
                }
            }

            return null;

        }

    }

}