#region Using Directives

using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Language.Gd.Extension.ParserService;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Outlining.OutlineTagger {

    class MultilineCommentOutlineTagger {

        public static IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot, IOutliningRegionTagCreator tagCreator) {

            foreach (var mcTrivia in syntaxTreeAndSnapshot.SyntaxTree
                                                          .Root.DescendantTokens()
                                                          .SelectMany(token => token.LeadingTrivia.Concat(token.TrailingTrivia))
                                                          .Where(t => t.Kind == SyntaxKind.MultiLineCommentTrivia)) {

                var extent = mcTrivia.Extent;

                if (extent.IsEmptyOrMissing) {
                    continue;
                }

                var startLine = syntaxTreeAndSnapshot.Snapshot.GetLineNumberFromPosition(extent.Start);
                var endLine   = syntaxTreeAndSnapshot.Snapshot.GetLineNumberFromPosition(extent.End);
                if (startLine == endLine) {
                    continue;
                }

                var collapsedForm = "/* ...";
                var rgnSpan       = new SnapshotSpan(new SnapshotPoint(syntaxTreeAndSnapshot.Snapshot, extent.Start), extent.Length);
                var hintSpan      = new SnapshotSpan(new SnapshotPoint(syntaxTreeAndSnapshot.Snapshot, extent.Start), extent.Length);
                var rgnTag        = tagCreator.CreateTag(collapsedForm, hintSpan);

                yield return new TagSpan<IOutliningRegionTag>(rgnSpan, rgnTag);
            }
        }

    }

}