#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Language.Gd.Extension.ParserService;
using Pharmatechnik.Language.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Classification {

    sealed class SyntacticClassificationTagger: ParserServiceDependent, ITagger<IClassificationTag>, IDisposable {

        readonly ImmutableDictionary<GdClassification, IClassificationType> _classificationMap;

        public SyntacticClassificationTagger(IClassificationTypeRegistryService registry, ITextBuffer textBuffer): base(textBuffer) {

            _classificationMap = ClassificationTypeDefinitions.GetClassificationMap(registry);
        }

        protected override void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {
            TagsChanged?.Invoke(this, new SnapshotSpanEventArgs(e.Span));
        }

        public IEnumerable<ITagSpan<IClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            var syntaxTreeAndSnapshot = ParserService.SyntaxTreeAndSnapshot;

            if (syntaxTreeAndSnapshot == null) {
                yield break;
            }

            foreach (var span in spans) {

                var extent = TextExtent.FromBounds(span.Start.Position, span.End.Position);
                var result = SyntaxClassifier.Classify(syntaxTreeAndSnapshot.SyntaxTree.Root, extent);

                foreach (var classifiedExtent in result) {

                    _classificationMap.TryGetValue(classifiedExtent.Classification, out var ct);
                    if (ct == null) {
                        continue;
                    }

                    var tokenSpan = new SnapshotSpan(syntaxTreeAndSnapshot.Snapshot, new Span(classifiedExtent.Extent.Start, classifiedExtent.Extent.Length));
                    var tagSpan   = tokenSpan.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive);
                    var tag       = new ClassificationTag(ct);

                    yield return new TagSpan<IClassificationTag>(tagSpan, tag);
                }
            }
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

    }

}