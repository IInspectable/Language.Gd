#region Using Directives

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion;
using Microsoft.VisualStudio.Language.Intellisense.AsyncCompletion.Data;
using Microsoft.VisualStudio.Text;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Completion {

    class CompletionCommitManager: IAsyncCompletionCommitManager {

        public static readonly string ReplacementTrackingSpanProperty = nameof(ReplacementTrackingSpanProperty);

        // TODO PotentialCommitCharacters aud Sinnhaftigkeit prüfen
        readonly ImmutableArray<char> _commitChars = new[] {
            ' ',
            '\'',
            '"',
            ',',
            '=',
            '+',
            '-',
            '@',
            '[', 
            ']',
        }.ToImmutableArray();

        public bool ShouldCommitCompletion(IAsyncCompletionSession session, SnapshotPoint location, char typedChar, CancellationToken token) {
            return true;
        }

        public CommitResult TryCommit(IAsyncCompletionSession session, ITextBuffer buffer, CompletionItem item, char typedChar, CancellationToken token) {
            if (item.Properties.TryGetProperty<ITrackingSpan>(ReplacementTrackingSpanProperty, out var replacementSpan)) {

                using (var edit = buffer.CreateEdit()) {

                    edit.Replace(replacementSpan.GetSpan(buffer.CurrentSnapshot), item.InsertText);
                    edit.Apply();

                    return CommitResult.Handled;
                }
            }

            return CommitResult.Unhandled; // use default commit mechanism.
        }

        public IEnumerable<char> PotentialCommitCharacters => _commitChars;

    }

}