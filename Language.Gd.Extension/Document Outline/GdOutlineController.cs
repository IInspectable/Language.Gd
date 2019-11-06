#region Using Directives

using System;
using System.Reactive.Linq;
using System.Threading;

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

        bool IsRunning { get; set; }

        public bool HasActiveTextView => _activeWpfTextView != null;

        public void Run() {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (IsRunning) {
                return;
            }

            IsRunning = true;

            ConnectRunningDocumentTable();
            SetActiveTextView(TryGetActiveGdTextView());
        }

        public void Stop() {

            ThreadHelper.ThrowIfNotOnUIThread();

            if (!IsRunning) {
                return;
            }

            SetActiveTextView(null);
            DisconnectRunningDocumentTable();

            IsRunning = false;
        }

        readonly ScopedProperty<bool> _isNavigatingToSource = ScopedProperty.Boolean();

        public void NavigateToSource(OutlineElement outlineElement) {

            if (_activeWpfTextView == null ||
                outlineElement     == null) {
                return;
            }

            using (_isNavigatingToSource.Enter()) {

                var snapshotPoint = new SnapshotPoint(_activeWpfTextView.TextBuffer.CurrentSnapshot,
                                                      outlineElement.NavigationPoint);

                var snapShotSpan = snapshotPoint.GetContainingLine().Extent;

                _activeWpfTextView.Caret.MoveTo(snapshotPoint);
                _activeWpfTextView.ViewScroller.EnsureSpanVisible(snapShotSpan);
            }

        }

        public void Dispose() {
            ThreadHelper.ThrowIfNotOnUIThread();
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

        string _searchString;

        public String SearchString {
            get => _searchString;
            set {

                if (_searchString == value) {
                    return;
                }

                _searchString=value;
                RaiseOutlineDataChanged();
            }
        }

        public void OnSearchOptionsChanged() {
            RaiseOutlineDataChanged();
        }

        void OnParseResultChanged(object sender, SnapshotSpanEventArgs e) {
            RaiseOutlineDataChanged();
        }

        void OnCaretPositionChanged(CaretPositionChangedEventArgs e) {
            RaiseRequestNavigateToOutline();
        }

        void RaiseOutlineDataChanged() {

            if (!IsRunning) {
                return;
            }

            OutlineDataChanged?.Invoke(this, new OutlineDataEventArgs(TryGetOutlineData(), SearchString));
            RaiseRequestNavigateToOutline();
        }

        void RaiseRequestNavigateToOutline() {

            if (_isNavigatingToSource || !IsRunning) {
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

        IDisposable _caretPositionEvents;

        void SetActiveTextView([CanBeNull] IWpfTextView wpfTextView) {

            if (_activeWpfTextView == wpfTextView || !IsRunning) {
                return;
            }
            
            // Disconnect from current view
            if (_activeWpfTextView != null) {

                _caretPositionEvents?.Dispose();
                _caretPositionEvents = null;

                ParserService.ParserService.GetOrCreateSingelton(_activeWpfTextView.TextBuffer).ParseResultChanged -= OnParseResultChanged;

                _activeWpfTextView = null;
                // TODO evtl. Searchtext im Buffer speichern
                //_searchString = null;
            }

            // Connect to active view
            if (wpfTextView != null && IsGdContentType(wpfTextView)) {

                _activeWpfTextView = wpfTextView;

                _caretPositionEvents = Observable.FromEventPattern<CaretPositionChangedEventArgs>(
                                                      addHandler: handler => wpfTextView.Caret.PositionChanged    += handler,
                                                      removeHandler: handler => wpfTextView.Caret.PositionChanged -= handler
                                                  )
                                                 .Throttle(ServiceProperties.OutlineControllerSyncThrottleTime)
                                                 .Select(d => d.EventArgs)
                                                 .ObserveOn(SynchronizationContext.Current)
                                                 .Subscribe(OnCaretPositionChanged);

                ParserService.ParserService.GetOrCreateSingelton(_activeWpfTextView.TextBuffer).ParseResultChanged += OnParseResultChanged;
            }

            RaiseOutlineDataChanged();
        }

        OutlineData _cachedOutlineData;

        [CanBeNull]
        OutlineData TryGetOutlineData() {

            var sts = TryGetActiveParserService()?.SyntaxTreeAndSnapshot;

            if (_cachedOutlineData != null && _cachedOutlineData.Snapshot == sts?.Snapshot) {
                return _cachedOutlineData;
            }

            var syntaxTree     = sts?.SyntaxTree;
            var snapshot       = sts?.Snapshot;
            var outlineElement = OutlineBuilder.Build(syntaxTree?.Root, detailed: true);

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