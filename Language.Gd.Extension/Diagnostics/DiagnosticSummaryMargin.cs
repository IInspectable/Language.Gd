#region Using Directives

using System;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Language.Gd.Extension.Imaging;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Diagnostics {

    sealed class DiagnosticSummaryMargin: IWpfTextViewMargin {

        #region Ctor / Fields

        const string TxtWaitingForAnalysisToFinish = "Waiting for analysis to finish";
        const string TxtNoErrorsOrWarnings         = "No errors or warnings";
        const string TxtClickToNavigateToNext0     = "Click to navigate to the next {0}";

        readonly IWpfTextView             _textView;
        readonly DiagnosticSummaryControl _summaryControl;
        readonly DiagnosticService        _diagnosticService;

        bool _isDisposed;

        public DiagnosticSummaryMargin(IWpfTextView textView) {

            _textView          = textView;
            _isDisposed        = false;
            _diagnosticService = DiagnosticService.GetOrCreate(textView);

            _summaryControl = new DiagnosticSummaryControl {
                CrispImage = {
                    Moniker = GetImageMoniker()
                }
            };
            _summaryControl.MouseLeftButtonDown += OnMouseLeftButtonDown;

            _diagnosticService.DiagnosticsChanging += OnDiagnosticsChanging;
            _diagnosticService.DiagnosticsChanged  += OnDiagnosticsChanged;
            _textView.Options.OptionChanged        += OnTextViewOptionChanged;
            _textView.Closed                       += OnTextViewClosed;

            UpdateVisibility();
            UpdateSummary();
        }

        #endregion

        public const string MarginName = GdLanguageContentDefinitions.ContentType + nameof(DiagnosticSummaryMargin);

        public FrameworkElement VisualElement {
            get {
                ThrowIfDisposed();
                return _summaryControl;
            }
        }

        public double MarginSize {
            get {
                ThrowIfDisposed();
                return _summaryControl.ActualWidth;
            }
        }

        public bool Enabled {
            get {
                ThrowIfDisposed();
                return _textView.Options.GetOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId);
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName) {
            return string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;

                _summaryControl.MouseLeftButtonDown -= OnMouseLeftButtonDown;

                _textView.Options.OptionChanged        -= OnTextViewOptionChanged;
                _textView.Closed                       -= OnTextViewClosed;
                _diagnosticService.DiagnosticsChanging -= OnDiagnosticsChanging;
                _diagnosticService.DiagnosticsChanged  -= OnDiagnosticsChanged;
            }
        }

        void ThrowIfDisposed() {
            if (_isDisposed) {
                throw new ObjectDisposedException(MarginName);
            }
        }

        void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {

            var shiftCtrlModifier = ModifierKeys.Control | ModifierKeys.Shift;

            if (Keyboard.Modifiers == ModifierKeys.None) {
                _diagnosticService.GoToNextDiagnostic();

            } else if ((Keyboard.Modifiers & shiftCtrlModifier) == shiftCtrlModifier && e.ClickCount == 2) {

                // Shift + Ctrl + Doubleclick 

                // Kopieren der Diagnostics in die Zwischenablage für Unittests
                var errors      = _diagnosticService.GetDiagnosticsWithSeverity(DiagnosticSeverity.Error).Select(d => d.Tag.Diagnostic);
                var warnings    = _diagnosticService.GetDiagnosticsWithSeverity(DiagnosticSeverity.Warning).Select(d => d.Tag.Diagnostic);
                var suggestions = _diagnosticService.GetDiagnosticsWithSeverity(DiagnosticSeverity.Suggestion).Select(d => d.Tag.Diagnostic);
                var diagnostics = errors.Concat(warnings)
                                        .Concat(suggestions)
                                        .Aggregate(
                                             new StringBuilder(),
                                             (sb, diagnostic) => sb.AppendLine(diagnostic.ToString(UnitTestDiagnosticFormatter.Instance)),
                                             sb => sb.ToString());

                Clipboard.SetText(diagnostics);

                // Neuberechnung der Diagnostik
                _diagnosticService.Invalidate();

            }
        }

        void OnTextViewClosed(object sender, EventArgs e) {
            Dispose();
        }

        void OnDiagnosticsChanging(object sender, EventArgs e) {
            UpdateSummary();
        }

        void OnDiagnosticsChanged(object sender, EventArgs e) {
            UpdateSummary();
        }

        void OnTextViewOptionChanged(object sender, EditorOptionChangedEventArgs args) {
            UpdateVisibility();
        }

        void UpdateVisibility() {
            _summaryControl.Visibility = Enabled ? Visibility.Visible : Visibility.Collapsed;
        }

        void UpdateSummary() {

            _summaryControl.ToolTip            = GetToolTipText();
            _summaryControl.CrispImage.Moniker = GetImageMoniker();
            _summaryControl.Cursor             = GetCursor();
        }

        Cursor GetCursor() {
            return _diagnosticService.CanGoToNextDiagnostic ? Cursors.Hand : null;
        }

        string GetToolTipText() {

            if (_diagnosticService.WaitingForAnalysis) {
                return TxtWaitingForAnalysisToFinish;
            }

            if (_diagnosticService.NoErrorsOrWarnings) {
                _summaryControl.Cursor = null;
                return TxtNoErrorsOrWarnings;
            }

            var    tipText   = new StringBuilder();
            string separator = "";

            var errorCount = _diagnosticService.CountDiagnosticsWithSeverity(DiagnosticSeverity.Error);
            if (errorCount > 0) {
                tipText.Append($"{errorCount} {GetDisplayName(DiagnosticSeverity.Error, errorCount)}");
                separator = " and ";
            }

            int warningCount = _diagnosticService.CountDiagnosticsWithSeverity(DiagnosticSeverity.Warning);
            if (warningCount > 0) {
                tipText.Append($"{separator}{warningCount} {GetDisplayName(DiagnosticSeverity.Warning, warningCount)}");
            }

            tipText.Append(Environment.NewLine);
            tipText.AppendFormat(TxtClickToNavigateToNext0, GetDisplayName(_diagnosticService.WorstSeverity.GetValueOrDefault(), 1));

            return tipText.ToString();
        }

        string GetDisplayName(DiagnosticSeverity severity, int count) {
            var numerus = count > 1 ? "s" : "";
            return severity.ToString().ToLower() + numerus;
        }

        ImageMoniker GetImageMoniker() {

            if (_diagnosticService.WaitingForAnalysis) {
                return ImageMonikers.WaitingForAnalysis;
            }

            var severity = _diagnosticService.WorstSeverity;
            return GetImageMoniker(severity);
        }

        static ImageMoniker GetImageMoniker(DiagnosticSeverity? severity) {
            switch (severity) {
                default:
                case DiagnosticSeverity.Suggestion:
                    return ImageMonikers.AnalysisOK;
                case DiagnosticSeverity.Warning:
                    return ImageMonikers.AnalysisWarning;
                case DiagnosticSeverity.Error:
                    return ImageMonikers.AnalysisError;
            }
        }

    }

}