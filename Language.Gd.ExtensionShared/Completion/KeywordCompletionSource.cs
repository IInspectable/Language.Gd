#region Using Directives

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Immutable;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

using Task = System.Threading.Tasks.Task;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Completion {

    class KeywordCompletionSource: IAsyncCompletionSource {

        private readonly ITextView _textView;

        public KeywordCompletionSource(ITextView textView) {
            _textView = textView;

        }

        public CompletionStartData InitializeCompletion(CompletionTrigger trigger, SnapshotPoint triggerLocation, CancellationToken token) {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_textView.Selection.Mode == TextSelectionMode.Box) {
                // No completion with multiple selection
                return CompletionStartData.DoesNotParticipateInCompletion;
            }

            if (!ShouldTriggerCompletion(trigger)) {
                return CompletionStartData.DoesNotParticipateInCompletion;
            }

            var guiDescriptionSyntax = GetSyntax(triggerLocation);

            if (ShouldProvideCompletions(triggerLocation, guiDescriptionSyntax, out var applicableToSpan)) {
                return new CompletionStartData(CompletionParticipation.ProvidesItems, applicableToSpan);
            }

            return CompletionStartData.DoesNotParticipateInCompletion;
        }

        bool ShouldTriggerCompletion(CompletionTrigger trigger) {
            // The trigger reason guarantees that user wants a completion.
            if (trigger.Reason == CompletionTriggerReason.Invoke ||
                trigger.Reason == CompletionTriggerReason.InvokeAndCommitIfUnique) {
                return true;
            }

            // Enter does not trigger completion.
            if (trigger.Reason == CompletionTriggerReason.Insertion && trigger.Character == '\n') {
                return false;
            }

            return char.IsLetter(trigger.Character);
        }

        public Task<CompletionContext> GetCompletionContextAsync(IAsyncCompletionSession session, CompletionTrigger trigger, SnapshotPoint triggerLocation, SnapshotSpan applicableToSpan, CancellationToken token) {

            var completionItems = ImmutableArray.CreateBuilder<CompletionItem>();
            foreach (var keyword in SyntaxFacts.Keywords.OrderBy(k => k)) {
                completionItems.Add(CreateKeywordCompletion(keyword));
            }

            return Task.FromResult(new CompletionContext(completionItems.ToImmutable(), null, InitialSelectionHint.SoftSelection));
        }

        public Task<object> GetDescriptionAsync(IAsyncCompletionSession session, CompletionItem item, CancellationToken token) {

            if (item.Properties.TryGetProperty<string>(KeywordDescriptionPropertyName, out var keyword)) {
                return Task.FromResult((object) (keyword + " Keyword"));
            }

            return Task.FromResult((object) item.DisplayText);
        }

        protected CompletionItem CreateKeywordCompletion(string keyword) {

            var completionItem = new CompletionItem(displayText: keyword,
                                                    source: this,
                                                    icon: CompletionImages.Keyword,
                                                    filters: ImmutableArray.Create(CompletionFilters.Keywords)
            );

            completionItem.Properties.AddProperty(KeywordDescriptionPropertyName, keyword);

            return completionItem;
        }

        public static string KeywordDescriptionPropertyName => nameof(KeywordDescriptionPropertyName);

        bool ShouldProvideCompletions(SnapshotPoint triggerLocation, GuiDescriptionSyntax guiDescriptionSyntax, out SnapshotSpan applicableToSpan) {

            applicableToSpan = default;

            // TODO FindTokenOrTrivia Methode?
            var triggerToken = guiDescriptionSyntax.FindToken(triggerLocation);
            if (triggerToken.IsMissing || triggerToken == default) {
                return false;
            }

            int start = triggerToken.ExtentStart;

            // Wir haben ein Token an der Trigger Location
            if (triggerToken.Extent.IntersectsWith(triggerLocation)) {
                // Keine Autovervollständigung in Zeichenfolgen
                if (triggerToken.Kind == SyntaxKind.String) {
                    return false;
                }
            } else {
                // Die Trigger Location befindet sich in den Trivias
                var tokenExtent  = triggerToken.Extent;
                var syntaxTrivia = default(SyntaxTrivia);

                if (triggerLocation < tokenExtent.Start) {
                    syntaxTrivia = triggerToken.LeadingTrivia.FirstOrDefault(t => t.Extent.IntersectsWith(triggerLocation));
                } else if (triggerLocation > tokenExtent.End) {
                    syntaxTrivia = triggerToken.TrailingTrivia.FirstOrDefault(t => t.Extent.IntersectsWith(triggerLocation));
                }

                // Kann es das geben?
                if (syntaxTrivia == default) {
                    return false;
                }

                // Keine Vervollständigung in Kommentaren
                if (syntaxTrivia.Kind == SyntaxKind.MultiLineCommentTrivia ||
                    syntaxTrivia.Kind == SyntaxKind.SingleLineCommentTrivia
                ) {
                    return false;
                }

                start = syntaxTrivia.ExtentStart;

            }

            applicableToSpan = new SnapshotSpan(new SnapshotPoint(triggerLocation.Snapshot, start), triggerLocation);
            return true;
        }

        private static GuiDescriptionSyntax GetSyntax(SnapshotPoint triggerLocation) {

            ThreadHelper.ThrowIfNotOnUIThread();

            var parserService = ParserService.ParserService.GetOrCreateSingelton(triggerLocation.Snapshot.TextBuffer);

            var generationUnitAndSnapshot = parserService.ParseSynchronously();
            var codeGenerationUnit        = generationUnitAndSnapshot.SyntaxTree.Root as GuiDescriptionSyntax;

            return codeGenerationUnit;
        }

    }

}