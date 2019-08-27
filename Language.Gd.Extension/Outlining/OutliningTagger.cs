#region Using Directives

using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Language.Gd.Extension.Common;
using Pharmatechnik.Language.Gd.Extension.ParserService;
using Pharmatechnik.Language.Gd.Outlining;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Outlining {

    sealed class OutliningTagger: ParserServiceDependent, ITagger<IOutliningRegionTag> {

        readonly List<ITagSpan<IOutliningRegionTag>> _outliningRegionTags;
        readonly CodeContentControlProvider          _codeContentControlProvider;

        OutliningTagger(ITextBuffer textBuffer, CodeContentControlProvider codeContentControlProvider): base(textBuffer) {

            _outliningRegionTags        = new List<ITagSpan<IOutliningRegionTag>>();
            _codeContentControlProvider = codeContentControlProvider;
        }

        public static ITagger<T> GetOrCreateSingelton<T>(ITextBuffer textBuffer, CodeContentControlProvider codeContentControlProvider) where T : ITag {
            return new TextBufferScopedTagger<T>(
                textBuffer: textBuffer,
                key: typeof(OutliningTagger),
                createFunc: () => new OutliningTagger(textBuffer, codeContentControlProvider) as ITagger<T>);
        }

        public event EventHandler<SnapshotSpanEventArgs> TagsChanged;

        public IEnumerable<ITagSpan<IOutliningRegionTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
            return _outliningRegionTags;
        }

        protected override void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {
            var syntaxTreeAndSnapshot = ParserService.SyntaxTreeAndSnapshot;
            if (syntaxTreeAndSnapshot == null) {
                return;
            }

            UpdateOutliningRegionTags(syntaxTreeAndSnapshot);

            TagsChanged?.Invoke(this, e);
        }

        void UpdateOutliningRegionTags(SyntaxTreeAndSnapshot syntaxTreeAndSnapshot) {

            _outliningRegionTags.Clear();

            var outliningDefinitions = OutliningProvider.Default.GetOutliningDefinitions(syntaxTreeAndSnapshot.SyntaxTree.Root);

            _outliningRegionTags.AddRange(ToRegionTags(outliningDefinitions, syntaxTreeAndSnapshot));
        }

        IEnumerable<ITagSpan<IOutliningRegionTag>> ToRegionTags(IEnumerable<OutliningDefinition> outliningDefinitions, SyntaxTreeAndSnapshot syntaxTreeAndSnapshot) {

            foreach (var outliningDefinition in outliningDefinitions) {

                var foldingExtent = outliningDefinition.FoldingExtent;
                var previewExtent = outliningDefinition.PreviewExtent;

                var foldingSpan = new SnapshotSpan(new SnapshotPoint(syntaxTreeAndSnapshot.Snapshot, foldingExtent.Start), length: foldingExtent.Length);
                var previewSpan = new SnapshotSpan(new SnapshotPoint(syntaxTreeAndSnapshot.Snapshot, previewExtent.Start), length: previewExtent.Length);

                var tag = new OutliningRegionTag(
                    isDefaultCollapsed: outliningDefinition.IsDefaultCollapsed,
                    isImplementation  : outliningDefinition.IsImplementation,
                    collapsedForm     : outliningDefinition.CollapsedText,
                    collapsedHintForm : _codeContentControlProvider.CreateContentControlForOutlining(previewSpan));

                yield return new TagSpan<IOutliningRegionTag>(foldingSpan, tag);
            }
        }

    }

}