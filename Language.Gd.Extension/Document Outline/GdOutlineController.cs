#region Using Directives

using System;

using JetBrains.Annotations;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Language.Gd.DocumentOutline;
using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    class GdOutlineController: IVsRunningDocTableEvents, IDisposable {

        IWpfTextView _activeWpfTextView;
        bool         _isRunning;

        public void Run() {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (_isRunning) {
                return;
            }

            _isRunning = true;

            ConnectRunningDocumentTable();
            SetActiveTextView(TryGetActiveGdTextView());
        }

        public void Stop() {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (!_isRunning) {
                return;
            }

            SetActiveTextView(null);
            DisconnectRunningDocumentTable();

            _isRunning = false;
        }

        private bool IsNavigatingToSource { get; set; }

        IDisposable NavigatingToSource() {
            return new ScopedValue<bool>(() => IsNavigatingToSource, v => IsNavigatingToSource = v, true);
        }

        public void NavigateToSource(OutlineElement outlineElement) {
            using (NavigatingToSource()) {
                if (_activeWpfTextView != null && outlineElement != null) {

                    var snapshotPoint = new SnapshotPoint(_activeWpfTextView.TextBuffer.CurrentSnapshot,
                                                          outlineElement.NavigationPoint);

                    var snapShotSpan = snapshotPoint.GetContainingLine().Extent;

                    _activeWpfTextView.Caret.MoveTo(snapshotPoint);
                    _activeWpfTextView.ViewScroller.EnsureSpanVisible(snapShotSpan);
                }
            }

        }

        public void Invalidate() {
            if (!_isRunning) {
                return;
            }

            RaiseOutlineDataChanged();
        }

        public void Dispose() {
            Stop();
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

        public event EventHandler<OutlineDataEventArgs>       OutlineDataChanged;
        public event EventHandler<NavigateToOutlineEventArgs> RequestNavigateToOutline;

        void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {
            Invalidate();
        }

        void OnCaretPositionChanged(object sender, CaretPositionChangedEventArgs e) {
            // TODO Throtteling?
            RaiseRequestNavigateToOutline();
        }

        private void RaiseOutlineDataChanged() {

            OutlineDataChanged?.Invoke(this, new OutlineDataEventArgs(TryGetOutlineData()));
            RaiseRequestNavigateToOutline();
        }

        void RaiseRequestNavigateToOutline() {

            if (IsNavigatingToSource) {
                return;
            }

            var rootElement = TryGetOutlineData()?.OutlineElement;
            if (rootElement == null) {
                return;
            }

            int caretPosition = _activeWpfTextView.Caret.Position.BufferPosition;
            var bestMatch     = rootElement.FindBestMatch(caretPosition);

            RequestNavigateToOutline?.Invoke(this, new NavigateToOutlineEventArgs(bestMatch));
        }

        private void SetActiveTextView([CanBeNull] IWpfTextView wpfTextView) {

            if (_activeWpfTextView == wpfTextView || !_isRunning) {
                return;
            }

            // Disconnect from current view
            if (_activeWpfTextView != null) {

                _activeWpfTextView.Caret.PositionChanged -= OnCaretPositionChanged;

                ParserService.ParserService.GetOrCreateSingelton(_activeWpfTextView.TextBuffer).ParseResultChanged -= OnParseResultChanged;

                _activeWpfTextView = null;
            }

            if (IsGdContentType(wpfTextView)) {
                _activeWpfTextView = wpfTextView;
            }

            // Connect to active view
            if (_activeWpfTextView != null) {
                _activeWpfTextView.Caret.PositionChanged += OnCaretPositionChanged;

                ParserService.ParserService.GetOrCreateSingelton(_activeWpfTextView.TextBuffer).ParseResultChanged += OnParseResultChanged;
            }

            Invalidate();
        }

        OutlineData _cachedOutlineData;

        [CanBeNull]
        private OutlineData TryGetOutlineData() {

            var sts = TryGetActiveParserService()?.SyntaxTreeAndSnapshot;

            if (_cachedOutlineData != null && _cachedOutlineData.Snapshot == sts?.Snapshot) {
                return _cachedOutlineData;
            }

            var syntaxTree     = sts?.SyntaxTree;
            var snapshot       = sts?.Snapshot;
            var outlineElement = OutlineBuilder.Build(syntaxTree?.Root);

            OutlineData outlineData = null;
            if (syntaxTree != null && outlineElement != null) {
                outlineData = new OutlineData(outlineElement, syntaxTree, snapshot);
            }

            _cachedOutlineData = outlineData;

            return outlineData;
        }

        ParserService.ParserService TryGetActiveParserService() {
            var activeTextBuffer = _activeWpfTextView?.TextBuffer;
            if (activeTextBuffer == null) {
                return null;
            }

            return ParserService.ParserService.TryGet(activeTextBuffer);
        }

        [CanBeNull]
        IWpfTextView TryGetActiveGdTextView() {

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