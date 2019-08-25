#region Using Directives

using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Language.Gd.Extension.ParserService;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Outlining.OutlineTagger {

    class SectionOutlineTagger {

        public static IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot, IOutliningRegionTagCreator tagCreator) {

            // TODO 
            foreach (var section in syntaxTreeAndSnapshot.SyntaxTree.Root
                                                         .DescendantNodes()
                                                         .OfType<ISectionSyntax>()) {

            }

            yield break;
        }

    }

}