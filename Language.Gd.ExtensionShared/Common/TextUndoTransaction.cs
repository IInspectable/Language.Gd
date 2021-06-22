#region Using Directives

using System;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    sealed class TextUndoTransaction: IDisposable {

        readonly    IEditorOperations    _editorOperations;
        [CanBeNull] ITextUndoTransaction _transaction;
        bool                             _inTransaction;

        public TextUndoTransaction(
            string description,
            ITextView textView,
            ITextUndoHistoryRegistry undoHistoryRegistry,
            IEditorOperationsFactoryService editorOperationsFactoryService) {

            _inTransaction    = true;
            _editorOperations = editorOperationsFactoryService.GetEditorOperations(textView);

            var undoHistory = undoHistoryRegistry.GetHistory(textView.TextBuffer);
            if (undoHistory != null) {
                _transaction = undoHistory.CreateTransaction(description);
                _editorOperations.AddBeforeTextBufferChangePrimitive();
            }
        }

        public void Dispose() {
            EndTransaction();
        }

        public void Commit() {
            if (!_inTransaction) {
                throw new InvalidOperationException("The transaction is already complete");
            }

            _editorOperations?.AddAfterTextBufferChangePrimitive();
            _transaction?.Complete();

            EndTransaction();
        }

        public void Cancel() {
            if (!_inTransaction) {
                throw new InvalidOperationException("The transaction is already complete");
            }

            _transaction?.Cancel();
            EndTransaction();
        }

        void EndTransaction() {
            _inTransaction = false;
            _transaction?.Dispose();
            _transaction = null;
        }

    }

}