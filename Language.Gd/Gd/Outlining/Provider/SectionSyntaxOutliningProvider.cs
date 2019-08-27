#region Using Directives

using System.Linq;
using System.Collections.Generic;

using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Outlining {

    class SectionSyntaxOutliningProvider: OutliningProvider {

        public override IEnumerable<OutliningDefinition> GetOutliningDefinitions(SyntaxNode root) {

            if (root == null) {
                yield break;
            }

            foreach (var section in root.DescendantNodes()
                                        .OfType<ISectionSyntax>()) {

                if (section.SectionBegin == null ||
                    section.SectionEnd   == null) {
                    continue;
                }

                var regionStartLine = section.SectionBegin.GetLocation().StartLine;
                var regionEndLine   = section.SectionEnd.GetLocation().EndLine;
                if (regionStartLine == regionEndLine) {
                    continue;
                }

                var foldingRegion = TextExtent.FromBounds(start: GetEndIncludingWhiteSpace(section.SectionBegin),
                                                          end: section.SectionEnd.Extent.End);

                var previewRegion = TextExtent.FromBounds(start: section.SectionBegin.ExtentStart,
                                                          end: section.SectionEnd.Extent.End);

                yield return new OutliningDefinition(collapsedText     : "...",
                                                     foldingExtent     : foldingRegion,
                                                     previewExtent     : previewRegion,
                                                     isDefaultCollapsed: false,
                                                     isImplementation  : true);
            }

            int GetEndIncludingWhiteSpace(SyntaxNode node) {

                var token    = node.LastToken();
                int position = token.Extent.End;

                foreach (var trivia in token.TrailingTrivia) {
                    if (trivia.Kind != SyntaxKind.WhitespaceTrivia) {
                        break;
                    }

                    position = trivia.End;
                }

                return position;
            }
        }

    }

}