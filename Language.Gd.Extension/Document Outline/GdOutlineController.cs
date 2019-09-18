#region Using Directives

using System;

using JetBrains.Annotations;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Language.Gd.DocumentOutline;
using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class GdOutlineController: IVsRunningDocTableEvents, IDisposable {

        IWpfTextView _activeWpfTextView;

        public void Run() {
            ThreadHelper.ThrowIfNotOnUIThread();
            ConnectRunningDocumentTable();
            SetActiveTextView(GetActiveGdTextView());
        }

        public void Dispose() {
            ThreadHelper.ThrowIfNotOnUIThread();
            SetActiveTextView(null);
            DisconnectRunningDocumentTable();
        }

        private uint _runningDocTableEventCookie;

        private void ConnectRunningDocumentTable() {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_runningDocTableEventCookie == 0) {
                RunningDocumentTable?.AdviseRunningDocTableEvents(this, out _runningDocTableEventCookie);
            }
        }

        private void DisconnectRunningDocumentTable() {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_runningDocTableEventCookie != 0) {
                RunningDocumentTable?.UnadviseRunningDocTableEvents(_runningDocTableEventCookie);
                _runningDocTableEventCookie = 0;
            }
        }

        IVsRunningDocumentTable RunningDocumentTable    => GdLanguagePackage.GetGlobalService<SVsRunningDocumentTable, IVsRunningDocumentTable>();
        IVsMonitorSelection     MonitorSelectionService => GdLanguagePackage.GetGlobalService<SVsShellMonitorSelection, IVsMonitorSelection>();

        public event EventHandler<GdOutlineEventArgs> OutlineDataChanged;

        private void OnSelectionChanged(object sender, EventArgs e) {
            OutlineDataChanged?.Invoke(this, new GdOutlineEventArgs(GetOutlineData()));
        }

        void Invalidate() {
            OutlineDataChanged?.Invoke(this, new GdOutlineEventArgs(GetOutlineData()));
        }

        [CanBeNull]
        private OutlineData GetOutlineData() {

            var sts            = TryGetActiveParserService()?.SyntaxTreeAndSnapshot;
            var syntaxTree     = sts?.SyntaxTree;
            var snapshot       = sts?.Snapshot;
            var outlineElement = OutlineBuilder.Build(syntaxTree?.Root);
            var position       = _activeWpfTextView?.Selection.ActivePoint.Position;

            OutlineData outlineData = null;
            if (syntaxTree != null && outlineElement != null) {
                outlineData = new OutlineData(outlineElement, position, syntaxTree, snapshot);
            }

            return outlineData;
        }

        

        private void SetActiveTextView([CanBeNull] IWpfTextView wpfTextView) {

            if (_activeWpfTextView == wpfTextView) {
                return;
            }

            // Disconnect from current view
            if (_activeWpfTextView != null) {

                _activeWpfTextView.Selection.SelectionChanged -= OnSelectionChanged;

                ParserService.ParserService.GetOrCreateSingelton(_activeWpfTextView.TextBuffer).ParseResultChanged -= OnParseResultChanged;

                _activeWpfTextView = null;
            }

            if (IsGdContentType(wpfTextView)) {
                _activeWpfTextView = wpfTextView;
            }

            // Connect to active view
            if (_activeWpfTextView != null) {

                _activeWpfTextView.Selection.SelectionChanged += OnSelectionChanged;

                ParserService.ParserService.GetOrCreateSingelton(_activeWpfTextView.TextBuffer).ParseResultChanged += OnParseResultChanged;
            }

            Invalidate();
        }

        ParserService.ParserService TryGetActiveParserService() {
            var activeTextBuffer = _activeWpfTextView?.TextBuffer;
            if (activeTextBuffer == null) {
                return null;
            }

            return ParserService.ParserService.TryGet(activeTextBuffer);
        }

        [CanBeNull]
        IWpfTextView GetActiveGdTextView() {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (!ErrorHandler.Succeeded(MonitorSelectionService.GetCurrentElementValue((uint) VSConstants.VSSELELEMID.SEID_DocumentFrame, out var value))) {
                return null;
            }

            if (value is IVsWindowFrame windowFrame) {

                var wpfTextView = windowFrame.ToWpfTextView();

                if (IsGdContentType(wpfTextView)) {
                    return wpfTextView;
                }

            }

            return null;
        }

        private void OnParseResultChanged(object sender, Microsoft.VisualStudio.Text.SnapshotSpanEventArgs e) {
            Invalidate();
        }

        bool IsGdContentType([CanBeNull] IWpfTextView wpfTextView) {
            return wpfTextView?.TextBuffer.ContentType?.IsOfType(GdLanguageContentDefinitions.ContentType) == true;
        }

        #region Implementation of IVsRunningDocTableEvents

        public int OnAfterFirstDocumentLock(uint docCookie, uint dwRdtLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) {
            return VSConstants.S_OK;
        }

        public int OnBeforeLastDocumentUnlock(uint docCookie, uint dwRdtLockType, uint dwReadLocksRemaining, uint dwEditLocksRemaining) {
            return VSConstants.S_OK;
        }

        public int OnAfterSave(uint docCookie) {
            return VSConstants.S_OK;
        }

        public int OnAfterAttributeChange(uint docCookie, uint grfAttribs) {
            return VSConstants.S_OK;
        }

        public int OnBeforeDocumentWindowShow(uint docCookie, int fFirstShow, IVsWindowFrame vsWindowFrame) {

            SetActiveTextView(vsWindowFrame.ToWpfTextView());

            return VSConstants.S_OK;
        }

        public int OnAfterDocumentWindowHide(uint docCookie, IVsWindowFrame vsWindowFrame) {

            if (_activeWpfTextView != null) {
                var wpfTextView = vsWindowFrame.ToWpfTextView();
                if (wpfTextView == _activeWpfTextView) {
                    SetActiveTextView(null);
                }
            }

            return VSConstants.S_OK;
        }

        #endregion

    }

}