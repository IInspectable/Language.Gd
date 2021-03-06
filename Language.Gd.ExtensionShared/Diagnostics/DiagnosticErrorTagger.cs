﻿#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Language.Gd.Extension.ParserService;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Diagnostics {

    sealed class DiagnosticErrorTagger: ParserServiceDependent /*SemanticModelServiceDependent,*/, ITagger<DiagnosticErrorTag> {

        public DiagnosticErrorTagger(ITextBuffer textBuffer): base(textBuffer) {
        }

        public IEnumerable<ITagSpan<DiagnosticErrorTag>> GetTags(NormalizedSnapshotSpanCollection spans) {

            var serviceSyntaxTreeAndSnapshot = ParserService.SyntaxTreeAndSnapshot;
            if (serviceSyntaxTreeAndSnapshot == null) {
                yield break;
            }

            var syntaxTree = serviceSyntaxTreeAndSnapshot.SyntaxTree;

            foreach (var span in spans) {

                //==================
                // Syntax Fehler
                foreach (var diagnostic in syntaxTree.Diagnostics.SelectMany(diag => diag.ExpandLocations())) {
                    if (diagnostic.Location.Start <= span.End && diagnostic.Location.End >= span.Start) {

                        var errorSpan = new SnapshotSpan(serviceSyntaxTreeAndSnapshot.Snapshot, new Span(diagnostic.Location.Start, diagnostic.Location.Length));

                        var errorTag = new TagSpan<DiagnosticErrorTag>(
                            errorSpan.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive),
                            new DiagnosticErrorTag(diagnostic));

                        yield return errorTag;
                    }
                }
            }

            // TODO DiagnosticErrorTagger GetTags Semantic Fehler

            //    //==================
            //    // Semantic Fehler
            //    foreach (var diagnostic in codeGenerationUnit.Diagnostics.SelectMany(diag => diag.ExpandLocations())) {
            //        if (diagnostic.Location.Start <= span.End && diagnostic.Location.End >= span.Start) {

            //            var errorSpan = new SnapshotSpan(codeGenerationUnitAndSnapshot.Snapshot, new Span(diagnostic.Location.Start, diagnostic.Location.Length));

            //            var errorTag = new TagSpan<DiagnosticErrorTag>(
            //                    errorSpan.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive),
            //                    new DiagnosticErrorTag(diagnostic));

            //            yield return errorTag;
            //        }
            //    }
            //}
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        protected override void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {
            TagsChanged?.Invoke(this, e);
        }
        // TODO OnSemanticModelChanged
        //protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs snapshotSpanEventArgs) {
        //    TagsChanged?.Invoke(this, snapshotSpanEventArgs);
        //}               //protected override void OnSemanticModelChanged(object sender, SnapshotSpanEventArgs snapshotSpanEventArgs) {
        //    TagsChanged?.Invoke(this, snapshotSpanEventArgs);
        //}       

    }

}