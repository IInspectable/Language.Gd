#region Using Directives

using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Threading;

using Pharmatechnik.Language.Gd.Extension.Imaging;
using Pharmatechnik.Language.Gd.Extension.ParserService;

using ThreadHelper = Microsoft.VisualStudio.Shell.ThreadHelper;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.QuickInfo {

    sealed class DebugQuickInfoSource: ParserServiceDependent, IAsyncQuickInfoSource {

        public DebugQuickInfoSource(ITextBuffer textBuffer): base(textBuffer) {
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

            var triggerToken = syntaxTreeAndSnapshot.SyntaxTree.Root.FindToken(triggerPoint.Value.Position);

            if (triggerToken.IsMissing || triggerToken.Parent == null) {
                return null;
            }

            var applicableToSpan = syntaxTreeAndSnapshot.Snapshot.CreateTrackingSpan(
                triggerToken.Extent.Start,
                triggerToken.Extent.Length,
                SpanTrackingMode.EdgeExclusive);

            var location  = triggerToken.GetLocation();
            var qiContent = $"{triggerToken.GetText()}\r\n{triggerToken.Kind} Ln {location?.StartLine + 1} Ch {location?.StartCharacter + 1}\r\n{triggerToken.Parent?.GetType().Name}";

            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var modifier = ModifierKeys.Control | ModifierKeys.Shift;
            if ((Keyboard.Modifiers & modifier) != modifier) {
                return null;
            }

            var controlControl = new SymbolQuickInfoControl {
                CrispImage  = {Moniker = ImageMonikers.StatusInformation},
                TextContent = {Content = qiContent}
            };

            var qiItem = new QuickInfoItem(applicableToSpan: applicableToSpan,
                                           item: controlControl
            );

            return qiItem;
        }

    }

}