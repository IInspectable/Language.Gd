#region Using Directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Pharmatechnik.Language.Gd.Outlining {

    class MultilineCommentOutliningProvider: OutliningProvider {

        public override IEnumerable<OutliningDefinition> GetOutliningDefinitions(SyntaxNode root) {

            if (root == null) {
                yield break;
            }

            foreach (var mcTrivia in root.DescendantTokens()
                                         .SelectMany(token => token.LeadingTrivia.Concat(token.TrailingTrivia))
                                         .Where(t => t.Kind == SyntaxKind.MultiLineCommentTrivia)) {

                var location = mcTrivia.GetLocation();
                var extent   = location.Extent;

                if (extent.IsEmptyOrMissing) {
                    continue;
                }

                if (location.StartLine == location.EndLine) {
                    continue;
                }

                yield return new OutliningDefinition(collapsedText     : "/* ...",
                                                     foldingExtent     : extent,
                                                     previewExtent     : extent,
                                                     isDefaultCollapsed: false,
                                                     isImplementation  : true);
            }
        }

    }

}