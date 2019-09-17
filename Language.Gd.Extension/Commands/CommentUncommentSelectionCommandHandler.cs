#region Using Directives

using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Commanding;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.Commanding.Commands;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;

using Pharmatechnik.Language.Gd.Extension.Common;
using Pharmatechnik.Language.Gd.Extension.Utilities;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Commands {

    [Export(typeof(ICommandHandler))]
    [ContentType(GdLanguageContentDefinitions.ContentType)]
    [Name(CommandHandlerNames.CommentUncommentSelectionCommandHandler)]
    class CommentUncommentSelectionCommandHandler: ICommandHandler<CommentSelectionCommandArgs>,
                                                   ICommandHandler<UncommentSelectionCommandArgs> {

        readonly IWaitIndicator                  _waitIndicator;
        readonly ITextUndoHistoryRegistry        _undoHistoryRegistry;
        readonly IEditorOperationsFactoryService _editorOperationsFactoryService;

        [ImportingConstructor]
        public CommentUncommentSelectionCommandHandler(
            IWaitIndicator waitIndicator,
            ITextUndoHistoryRegistry undoHistoryRegistry,
            IEditorOperationsFactoryService editorOperationsFactoryService) {

            _waitIndicator                  = waitIndicator;
            _undoHistoryRegistry            = undoHistoryRegistry;
            _editorOperationsFactoryService = editorOperationsFactoryService;
        }

        public string DisplayName => "Comment/uncomment lines";

        public CommandState GetCommandState(CommentSelectionCommandArgs args) {
            if (args.SubjectBuffer.CheckEditAccess()) {
                return CommandState.Available;
            }

            return CommandState.Unspecified;
        }

        public bool ExecuteCommand(CommentSelectionCommandArgs args, CommandExecutionContext executionContext) {

            ExecuteCommand(args.TextView, args.SubjectBuffer, Operation.Comment);
            return true;
        }

        public CommandState GetCommandState(UncommentSelectionCommandArgs args) {
            if (args.SubjectBuffer.CheckEditAccess()) {
                return CommandState.Available;
            }

            return CommandState.Unspecified;
        }

        public bool ExecuteCommand(UncommentSelectionCommandArgs args, CommandExecutionContext executionContext) {
            ExecuteCommand(args.TextView, args.SubjectBuffer, Operation.Uncomment);
            return true;
        }

        public CommandState GetCommandState(UncommentSelectionCommandArgs args, Func<CommandState> nextHandler) {
            if (args.SubjectBuffer.CheckEditAccess()) {
                return CommandState.Available;
            }

            return nextHandler();
        }

        public void ExecuteCommand(UncommentSelectionCommandArgs args, Action nextHandler) {
            ExecuteCommand(args.TextView, args.SubjectBuffer, Operation.Uncomment);
        }

        void ExecuteCommand(ITextView textView, ITextBuffer subjectBuffer, Operation operation) {

            var title   = operation == Operation.Comment ? "Comment Selection" : "Uncomment Selection";
            var message = operation == Operation.Comment ? "Commenting currently selected text..." : "Uncommenting currently selected text...";

            using (_waitIndicator.StartWait(title, message, allowCancel: false)) {

                using (var undoTransaction = new TextUndoTransaction(title, textView, _undoHistoryRegistry, _editorOperationsFactoryService))
                using (var textEdit = subjectBuffer.CreateEdit()) {

                    var spansToSelect = new List<ITrackingSpan>();
                    CollectEdits(textView.Options, textView.Selection.GetSnapshotSpansOnBuffer(subjectBuffer), textEdit, spansToSelect, operation);

                    textEdit.Apply();

                    if (spansToSelect.Any()) {
                        textView.SetSelection(spansToSelect.First().GetSpan(subjectBuffer.CurrentSnapshot));
                    }

                    undoTransaction.Commit();
                }
            }
        }

        void CollectEdits(IEditorOptions options, NormalizedSnapshotSpanCollection selectedSpans, ITextEdit textEdit, List<ITrackingSpan> spansToSelect, Operation operation) {
            foreach (var span in selectedSpans) {
                if (operation == Operation.Comment) {
                    CommentSpan(options, span, textEdit, spansToSelect);
                } else {
                    UncommentSpan(span, textEdit, spansToSelect);
                }
            }
        }

        void CommentSpan(IEditorOptions options, SnapshotSpan span, ITextEdit textEdit, List<ITrackingSpan> spansToSelect) {
            var firstAndLastLine = DetermineFirstAndLastLine(span);

            // Keine Selection, und in die ganze Zeile ist leer
            if (span.IsEmpty && firstAndLastLine.Item1.IsEmptyOrWhitespace()) {
                return;
            }

            // Blockselektion von leeren Zeilem
            if (!span.IsEmpty && string.IsNullOrWhiteSpace(span.GetText())) {
                return;
            }

            if (span.IsEmpty || string.IsNullOrWhiteSpace(span.GetText())) {
                var firstNonWhitespaceOnLine = firstAndLastLine.Item1.GetFirstNonWhitespacePosition();
                var insertPosition           = firstNonWhitespaceOnLine ?? firstAndLastLine.Item1.Start;

                // es gibt keine Selektion => ganze Zeile auskommentieren
                textEdit.Insert(insertPosition, SyntaxFacts.SingleLineComment);

                spansToSelect.Add(span.Snapshot.CreateTrackingSpan(Span.FromBounds(firstAndLastLine.Item1.Start, firstAndLastLine.Item1.End), SpanTrackingMode.EdgeInclusive));
            } else {
                // Partielle Selektion innerhalb einer Zeile
                if (!SpanIncludesAllTextOnIncludedLines(span) &&
                    firstAndLastLine.Item1.LineNumber == firstAndLastLine.Item2.LineNumber) {

                    textEdit.Insert(span.Start, SyntaxFacts.BlockCommentStart);
                    textEdit.Insert(span.End,   SyntaxFacts.BlockCommentEnd);

                    spansToSelect.Add(span.Snapshot.CreateTrackingSpan(span, SpanTrackingMode.EdgeInclusive));

                } else {

                    // Das Kommentare an der kleinsten Spalte beginnen, die nicht aus einem Leerzeichen besteht
                    // Bsp.:
                    // ...A
                    // ....B
                    // ->->C
                    // Kommentar beginnt bei Spalte 3
                    var indentToColumn = DetermineSmallestSignificantColumn(options, span, firstAndLastLine);
                    ApplyCommentToNonBlankLines(options, textEdit, firstAndLastLine, indentToColumn);

                    // Den ganzen "Block" an Zeilen selektieren
                    spansToSelect.Add(span.Snapshot.CreateTrackingSpan(Span.FromBounds(firstAndLastLine.Item1.Start.Position, firstAndLastLine.Item2.End.Position), SpanTrackingMode.EdgeInclusive));
                }
            }
        }

        bool TryUncommentSingleLineComments(SnapshotSpan span, ITextEdit textEdit, List<ITrackingSpan> spansToSelect) {
            // First see if we're selecting any lines that have the single-line comment prefix.
            // If so, then we'll just remove the single-line comment prefix from those lines.
            bool textChanges      = false;
            var  firstAndLastLine = DetermineFirstAndLastLine(span);

            for (int lineNumber = firstAndLastLine.Item1.LineNumber; lineNumber <= firstAndLastLine.Item2.LineNumber; ++lineNumber) {

                var line     = span.Snapshot.GetLineFromLineNumber(lineNumber);
                var lineText = line.GetText();

                if (lineText.Trim().StartsWith(SyntaxFacts.SingleLineComment, StringComparison.Ordinal)) {
                    textEdit.Delete(new Span(line.Start.Position + lineText.IndexOf(SyntaxFacts.SingleLineComment, StringComparison.Ordinal), SyntaxFacts.SingleLineComment.Length));
                    textChanges = true;
                }
            }

            // If we made any changes, select the entirety of the lines we change, so that subsequent invocations will
            // affect the same lines.
            if (!textChanges) {
                return false;
            }

            spansToSelect.Add(span.Snapshot.CreateTrackingSpan(
                                  Span.FromBounds(firstAndLastLine.Item1.Start.Position, firstAndLastLine.Item2.End.Position),
                                  SpanTrackingMode.EdgeExclusive));

            return true;
        }

        void TryUncommentContainingBlockComment(SnapshotSpan span, ITextEdit textEdit, List<ITrackingSpan> spansToSelect) {

            var positionOfStart = -1;
            var positionOfEnd   = -1;
            var spanText        = span.GetText();
            var trimmedSpanText = spanText.Trim();

            // See if the selection includes just a block comment (plus whitespace)
            if (trimmedSpanText.StartsWith(SyntaxFacts.BlockCommentStart, StringComparison.Ordinal) && trimmedSpanText.EndsWith(SyntaxFacts.BlockCommentEnd, StringComparison.Ordinal)) {
                positionOfStart = span.Start + spanText.IndexOf(SyntaxFacts.BlockCommentStart, StringComparison.Ordinal);
                positionOfEnd   = span.Start + spanText.LastIndexOf(SyntaxFacts.BlockCommentEnd, StringComparison.Ordinal);
            } else {
                // See if we are (textually) contained in a block comment.
                // This could allow a selection that spans multiple block comments to uncomment the beginning of
                // the first and end of the last.  Oh well.
                positionOfStart = span.Snapshot.LastIndexOf(SyntaxFacts.BlockCommentStart, span.Start, caseSensitive: true);

                // If we found a start comment marker, make sure there isn't an end comment marker after it but before our span.
                if (positionOfStart >= 0) {
                    var lastEnd = span.Snapshot.LastIndexOf(SyntaxFacts.BlockCommentEnd, span.Start, caseSensitive: true);
                    if (lastEnd < positionOfStart) {
                        positionOfEnd = span.Snapshot.IndexOf(SyntaxFacts.BlockCommentEnd, span.End, caseSensitive: true);
                    } else if (lastEnd + SyntaxFacts.BlockCommentEnd.Length > span.End) {
                        // The end of the span is *inside* the end marker, so searching backwards found it.
                        positionOfEnd = lastEnd;
                    }
                }
            }

            if (positionOfStart < 0 || positionOfEnd < 0) {
                return;
            }

            textEdit.Delete(new Span(positionOfStart, SyntaxFacts.BlockCommentStart.Length));
            textEdit.Delete(new Span(positionOfEnd,   SyntaxFacts.BlockCommentEnd.Length));

            spansToSelect.Add(span.Snapshot.CreateTrackingSpan(Span.FromBounds(positionOfStart, positionOfEnd + SyntaxFacts.BlockCommentEnd.Length), SpanTrackingMode.EdgeExclusive));
        }

        /// <summary> Adds edits to comment out each non-blank line, at the given column. </summary>
        void ApplyCommentToNonBlankLines(IEditorOptions options, ITextEdit textEdit, Tuple<ITextSnapshotLine, ITextSnapshotLine> firstAndLastLine, int indentToColumn) {
            for (int lineNumber = firstAndLastLine.Item1.LineNumber; lineNumber <= firstAndLastLine.Item2.LineNumber; ++lineNumber) {
                var line = firstAndLastLine.Item1.Snapshot.GetLineFromLineNumber(lineNumber);
                if (!line.IsEmptyOrWhitespace()) {

                    var offset = line.GetOffsetForColumn(indentToColumn, options.GetTabSize());
                    textEdit.Insert(line.Start + offset, SyntaxFacts.SingleLineComment);
                }
            }
        }

        void UncommentSpan(SnapshotSpan span, ITextEdit textEdit, List<ITrackingSpan> spansToSelect) {
            if (TryUncommentSingleLineComments(span, textEdit, spansToSelect)) {
                return;
            }

            TryUncommentContainingBlockComment(span, textEdit, spansToSelect);
        }

        static Tuple<ITextSnapshotLine, ITextSnapshotLine> DetermineFirstAndLastLine(SnapshotSpan span) {
            var firstLine = span.Snapshot.GetLineFromPosition(span.Start.Position);
            var lastLine  = span.Snapshot.GetLineFromPosition(span.End.Position);
            if (lastLine.Start == span.End.Position && !span.IsEmpty) {
                lastLine = lastLine.GetPreviousMatchingLine(_ => true);
            }

            return Tuple.Create(firstLine, lastLine);
        }

        /// <summary> Returns true if the span includes all of the non-whitespace text on the first and last line. </summary>
        static bool SpanIncludesAllTextOnIncludedLines(SnapshotSpan span) {
            var firstAndLastLine = DetermineFirstAndLastLine(span);

            var firstNonWhitespacePosition = firstAndLastLine.Item1.GetFirstNonWhitespacePosition();
            var lastNonWhitespacePosition  = firstAndLastLine.Item2.GetLastNonWhitespacePosition();

            var allOnFirst = !firstNonWhitespacePosition.HasValue ||
                             span.Start.Position <= firstNonWhitespacePosition.Value;
            var allOnLast = !lastNonWhitespacePosition.HasValue ||
                            span.End.Position > lastNonWhitespacePosition.Value;

            return allOnFirst && allOnLast;
        }

        /// <summary> Given a set of lines, find the minimum indent of all of the non-blank, non-whitespace lines.</summary>
        static int DetermineSmallestSignificantColumn(IEditorOptions options, SnapshotSpan span, Tuple<ITextSnapshotLine, ITextSnapshotLine> firstAndLastLine) {

            var tabSize           = options.GetTabSize();
            var indentToCommentAt = int.MaxValue;

            for (int lineNumber = firstAndLastLine.Item1.LineNumber; lineNumber <= firstAndLastLine.Item2.LineNumber; ++lineNumber) {

                var line = span.Snapshot.GetLineFromLineNumber(lineNumber);

                var significantColumn = line.GetIndentationColumn(tabSize) ?? Int32.MaxValue;
                indentToCommentAt = Math.Min(indentToCommentAt, significantColumn);
            }

            return indentToCommentAt;
        }

        enum Operation {

            Comment,
            Uncomment

        }

    }

}