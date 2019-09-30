#region Using Directives

using System.Linq;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;

using Pharmatechnik.Language.Gd.Extension.Common;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.SmartIndent {

    internal class GdSmartIndent: ISmartIndent {

        private readonly ITextView _textView;

        public GdSmartIndent(ITextView textView) {
            _textView = textView;

        }

        public void Dispose() {
        }

        private int TabSize => _textView.Options.GetTabSize();

        public int? GetDesiredIndentation(ITextSnapshotLine line) {

            // Default Einrückung ist die der vorigen Zeile
            var defaultIndent = GetPreviousNonEmptyLine(line)?.GetIndentationColumn(TabSize);

            // Die syntaktische Einrückung steht und fällt mit dem Beginn der Sektion
            var containingSection = TryGetContainingSectionOrSelf(line);
            if (containingSection?.SectionBegin == null) {
                return defaultIndent;
            }

            var sectionBeginIndentation = GetIndendation(containingSection.SectionBegin?.FirstToken(), TabSize) ?? defaultIndent;

            // SectionBegin: Parent Section + TabSize
            if (containingSection.SectionBegin.FullExtent.Contains(line.Start)) {

                var parentSection = containingSection.SectionBegin.Parent?.Ancestors()
                                                     .OfType<ISectionSyntax>()
                                                     .FirstOrDefault();

                return GetIndendation(parentSection?.SectionBegin?.FirstToken(), TabSize) + TabSize ?? defaultIndent;
            }

            // TODO: Wenn Kommentar, dann auf ebene der Section End einrücken. Nur Leere
            // Zeilen vor dem SectionEnd als potentiellen Section Body einrücken.
            var lineExtent = line.Extent.ToTextExtent();
            // SectionEnd: wie SectionBegin einrücken
            if (containingSection.SectionEnd?.Extent.IntersectsWith(lineExtent) ?? false) {
                return sectionBeginIndentation;
            }

            // SectionBody:  SectionBegin + TabSize
            return sectionBeginIndentation + TabSize;
        }

        int? GetIndendation(SyntaxToken? token, int tabsize) {

            var line = token?.SyntaxTree?.SourceText?.GetTextLineAtPosition(token.Value.ExtentStart);

            return line?.Span.GetColumnForOffset(tabsize, token.Value.ExtentStart - line.Value.Start);

        }

        ISectionSyntax TryGetContainingSectionOrSelf(ITextSnapshotLine line) {
            var syntaxTree = TryGetSyntaxTree(updateSyntax: true);

            var containingSection = syntaxTree?.Root
                                               .FindToken(line.Start)
                                               .Parent?
                                               .Ancestors()
                                               .OfType<ISectionSyntax>()
                                               .FirstOrDefault();

            return containingSection;
        }

        [CanBeNull]
        private SyntaxTree TryGetSyntaxTree(bool updateSyntax = false) {

            var textBuffer = _textView.GetBufferContainingCaret();
            if (textBuffer == null) return null;

            var parserService = ParserService.ParserService.TryGet(textBuffer);
            if (updateSyntax) {
                parserService.UpdateSynchronously();
            }

            return parserService?.SyntaxTreeAndSnapshot?.SyntaxTree;
        }

        [CanBeNull]
        private static ITextSnapshotLine GetPreviousNonEmptyLine(ITextSnapshotLine line) {

            var lineNumber = line.LineNumber;

            while (lineNumber > 0) {

                lineNumber--;
                var prevLine = line.Snapshot.GetLineFromLineNumber(lineNumber);
                if (!string.IsNullOrWhiteSpace(prevLine.GetText())) return prevLine;
            }

            return null;

        }

    }

}