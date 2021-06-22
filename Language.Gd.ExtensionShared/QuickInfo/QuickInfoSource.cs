#region Using Directives

using System.Threading;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Threading;

using Pharmatechnik.Language.Gd.Extension.Classification;
using Pharmatechnik.Language.Gd.Extension.Imaging;
using Pharmatechnik.Language.Gd.Extension.ParserService;
using Pharmatechnik.Language.Gd.QuickInfo;

using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.QuickInfo {

    sealed class QuickInfoSource: ParserServiceDependent, IAsyncQuickInfoSource {

        private readonly TextBlockBuilderService _textBlockBuilderService;

        public QuickInfoSource(ITextBuffer textBuffer, TextBlockBuilderService textBlockBuilderService): base(textBuffer) {
            _textBlockBuilderService = textBlockBuilderService;
        }

        public async Task<QuickInfoItem> GetQuickInfoItemAsync(IAsyncQuickInfoSession session, CancellationToken cancellationToken) {

            await Task.Yield().ConfigureAwait(false);

            if (cancellationToken.IsCancellationRequested) {
                return null;
            }

            var syntaxTreeAndSnapshot = ParserService.SyntaxTreeAndSnapshot;
            if (syntaxTreeAndSnapshot == null) {
                return null;
            }

            // Map the trigger point down to our buffer.
            SnapshotPoint? triggerPoint = session.GetTriggerPoint(syntaxTreeAndSnapshot.Snapshot);
            if (triggerPoint == null) {
                return null;
            }

            var qiInfo = QuickInfoProvider.GetQuickInfoDefinition(syntaxTreeAndSnapshot.SyntaxTree, triggerPoint.Value.Position);

            if (qiInfo == null) {
                return null;
            }

            var applicableToSpan = syntaxTreeAndSnapshot.Snapshot.CreateTrackingSpan(
                qiInfo.ApplicableToExtent.Start,
                qiInfo.ApplicableToExtent.Length,
                SpanTrackingMode.EdgeExclusive);

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var qiContent = _textBlockBuilderService.ToTextBlock(qiInfo.Content);

            var controlControl = new SymbolQuickInfoControl {
                CrispImage  = {Moniker = GdImageMonikers.GetMoniker(qiInfo.Glyph)},
                TextContent = {Content = qiContent}
            };

            var qiItem = new QuickInfoItem(applicableToSpan: applicableToSpan,
                                           item: controlControl
            );

            return qiItem;
        }

    }

}