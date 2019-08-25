#region Using Directives

using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Language.Gd.Extension.ParserService;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Outlining.OutlineTagger {

    class SectionOutlineTagger {

        public static IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot, IOutliningRegionTagCreator tagCreator) {

            foreach (var section in syntaxTreeAndSnapshot.SyntaxTree.Root
                                                         .DescendantNodes()
                                                         .OfType<ISectionSyntax>()) {

                if (section.SectionBegin == null ||
                    section.SectionEnd   == null) {
                    continue;
                }

                // TODO Letztes Trailing verwenden, wenn LastToken implementiert ist
                var regionExtent = TextExtent.FromBounds(start: section.SectionBegin.Extent.End,
                                                         end: section.SectionEnd.Extent.End);

                var hintExtent = TextExtent.FromBounds(start: section.SectionBegin.ExtentStart,
                                                       end: section.SectionEnd.Extent.End);

                // TODO Location der Syntax verwenden
                var regionStartLine = syntaxTreeAndSnapshot.Snapshot.GetLineNumberFromPosition(regionExtent.Start);
                var regionEndLone   = syntaxTreeAndSnapshot.Snapshot.GetLineNumberFromPosition(regionExtent.End);
                if (regionStartLine == regionEndLone) {
                    continue;
                }

                var regionSpan = new SnapshotSpan(new SnapshotPoint(syntaxTreeAndSnapshot.Snapshot, regionExtent.Start), length: regionExtent.Length);
                var hintSpan   = new SnapshotSpan(new SnapshotPoint(syntaxTreeAndSnapshot.Snapshot, hintExtent.Start),   length: hintExtent.Length);
                var tag        = tagCreator.CreateTag("...", hintSpan);

                yield return new TagSpan<IOutliningRegionTag>(regionSpan, tag);
            }

        }

    }

}