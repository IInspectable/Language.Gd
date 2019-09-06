#region Using Directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using JetBrains.Annotations;

using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Outlining;
using Microsoft.VisualStudio.Text.Tagging;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Diagnostics {

    sealed class DiagnosticService: IDisposable {

        readonly IWpfTextView                       _textView;
        readonly ITagAggregator<DiagnosticErrorTag> _errorTagAggregator;
        readonly IOutliningManagerService           _outliningManagerService;
        bool                                        _waitingForAnalysis;

        [NotNull]
        IReadOnlyDictionary<DiagnosticSeverity, ReadOnlyCollection<IMappingTagSpan<DiagnosticErrorTag>>> _diagnosticMapping;

        DiagnosticSeverity? _worstSeverity;

        DiagnosticService(IWpfTextView textView, IComponentModel componentModel) {
            var viewTagAggregatorFactoryService = componentModel.GetService<IViewTagAggregatorFactoryService>();

            _textView                = textView;
            _outliningManagerService = componentModel.GetService<IOutliningManagerService>();
            _errorTagAggregator      = viewTagAggregatorFactoryService.CreateTagAggregator<DiagnosticErrorTag>(textView);
            _diagnosticMapping       = new Dictionary<DiagnosticSeverity, ReadOnlyCollection<IMappingTagSpan<DiagnosticErrorTag>>>();
            _waitingForAnalysis      = true;

            _textView.Closed                       += OnTextViewClosed;
            _textView.TextBuffer.Changed           += OnTextBufferChanged;
            _errorTagAggregator.BatchedTagsChanged += OnBatchedTagsChanged;

            // Evtl. gibt es bereits einen Syntaxbaum...
            Invalidate();
        }

        public static DiagnosticService GetOrCreate(IWpfTextView textView) {
            var componentModel = GdLanguagePackage.GetGlobalService<SComponentModel, IComponentModel>();
            return textView.Properties.GetOrCreateSingletonProperty(() =>
                                                                        new DiagnosticService(textView, componentModel));
        }

        public void Dispose() {
            _textView.Properties.RemoveProperty(this);

            _textView.Closed                       -= OnTextViewClosed;
            _textView.TextBuffer.Changed           -= OnTextBufferChanged;
            _errorTagAggregator.BatchedTagsChanged -= OnBatchedTagsChanged;
            _errorTagAggregator?.Dispose();
        }

        void OnTextBufferChanged(object sender, TextContentChangedEventArgs e) {
            OnDiagnosticsChanging();
        }

        void OnTextViewClosed(object sender, EventArgs e) {
            Dispose();
        }

        void OnBatchedTagsChanged(object sender, BatchedTagsChangedEventArgs e) {
            UpdateDiagnostics();
        }

        public event EventHandler DiagnosticsChanging;

        void OnDiagnosticsChanging() {

            _waitingForAnalysis = true;

            DiagnosticsChanging?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler DiagnosticsChanged;

        void OnDiagnosticsChanged() {

            _waitingForAnalysis = false;

            DiagnosticsChanged?.Invoke(this, EventArgs.Empty);
        }

        [CanBeNull]
        public DiagnosticSeverity? WorstSeverity {
            get { return _worstSeverity; }
        }

        public bool NoErrorsOrWarnings {
            get {
                return CountDiagnosticsWithSeverity(DiagnosticSeverity.Error)   == 0 &&
                       CountDiagnosticsWithSeverity(DiagnosticSeverity.Warning) == 0;
            }
        }

        public bool WaitingForAnalysis {
            get { return _waitingForAnalysis; }
        }

        public void Invalidate() {

            OnDiagnosticsChanging();

            // TODO Invalidate
            //foreach(var modelService in _textView.BufferGraph
            //                                     .GetTextBuffers(tb => SemanticModelService.TryGet(tb) != null)
            //                                     .Select(SemanticModelService.TryGet)) {
            //    modelService?.Invalidate();
            //}
        }

        public bool HasDiagnosticsWithSeverity(DiagnosticSeverity severity) {
            return _diagnosticMapping.ContainsKey(severity);
        }

        public int CountDiagnosticsWithSeverity(DiagnosticSeverity severity) {
            return HasDiagnosticsWithSeverity(severity) ? _diagnosticMapping[severity].Count : 0;
        }

        public IEnumerable<IMappingTagSpan<DiagnosticErrorTag>> GetDiagnosticsWithSeverity(DiagnosticSeverity severity) {
            return (HasDiagnosticsWithSeverity(severity) ? _diagnosticMapping[severity] : null)
                ?? Enumerable.Empty<IMappingTagSpan<DiagnosticErrorTag>>();
        }

        public bool CanGoToNextDiagnostic {
            get { return _diagnosticMapping.Count > 0; }
        }

        public bool GoToNextDiagnostic() {

            var severities = new[] {
                DiagnosticSeverity.Error,
                DiagnosticSeverity.Warning,
                DiagnosticSeverity.Suggestion
            };

            return severities.Where(HasDiagnosticsWithSeverity)
                             .Select(GoToNextDiagnostic)
                             .FirstOrDefault();
        }

        public bool GoToNextDiagnostic(DiagnosticSeverity severity) {

            if (!HasDiagnosticsWithSeverity(severity)) {
                return false;
            }

            var caretPos = _textView.Caret.Position.BufferPosition;

            // TODO noch optimieren / überprüfen
            foreach (var tagSpan in GetDiagnosticsWithSeverity(severity)
               .Select(mappingTagSpan => _textView.MapToSingleSnapshotSpan(mappingTagSpan))) {

                if (tagSpan?.Span.Start > caretPos) {
                    return GoToDiagnostic(tagSpan);
                }
            }

            var firstMappingTagSpan = GetDiagnosticsWithSeverity(severity).First();
            var ts                  = _textView.MapToSingleSnapshotSpan(firstMappingTagSpan);
            return GoToDiagnostic(ts);
        }

        void UpdateDiagnostics() {

            var mappingSpan = _textView.BufferGraph.CreateMappingSpan(
                new SnapshotSpan(_textView.TextSnapshot, 0, _textView.TextSnapshot.Length),
                SpanTrackingMode.EdgeInclusive);

            var diagnosticMapping = _errorTagAggregator.GetTags(mappingSpan)
                                                       .GroupBy(tagSpan => tagSpan.Tag.Diagnostic.Severity)
                                                       .ToDictionary(
                                                            grouping => grouping.Key,
                                                            grouping => grouping.OrderBy(tags => tags.Tag.Diagnostic.Location.Start)
                                                                                .ToList()
                                                                                .AsReadOnly());

            _diagnosticMapping = diagnosticMapping;
            _worstSeverity     = diagnosticMapping.Keys.GetWorst();

            OnDiagnosticsChanged();
        }

        bool GoToDiagnostic(ITagSpan<DiagnosticErrorTag> tagSpan) {
            if (tagSpan == null) {
                return false;
            }

            return _textView.TryMoveCaretToAndEnsureVisible(tagSpan.Span.Start, _outliningManagerService);
        }

    }

}