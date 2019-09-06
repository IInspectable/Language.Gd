#region Using Directives

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;

using Pharmatechnik.Language.Gd.Extension.Common;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Diagnostics {

    sealed class DiagnosticStripeMargin: Border, IWpfTextViewMargin {

        readonly IWpfTextView _textView;
        bool                  _isDisposed;

        public const string MarginName = GdLanguageContentDefinitions.ContentType + nameof(DiagnosticStripeMargin);

        readonly DiagnosticService _diagnosticService;

        readonly IVerticalScrollBar _scrollBar;
        readonly IEditorFormatMap   _editorFormatMap;

        Dictionary<DiagnosticSeverity, GeometryGroup> _markerGeometryGroups;

        public DiagnosticStripeMargin(IWpfTextView textView, IVerticalScrollBar scrollBar,
                                      IEditorFormatMapService editorFormatMapService) {

            _textView          = textView;
            _isDisposed        = false;
            _scrollBar         = scrollBar;
            _editorFormatMap   = editorFormatMapService.GetEditorFormatMap(textView);
            _diagnosticService = DiagnosticService.GetOrCreate(textView);

            ClipToBounds      = true;
            Background        = null;
            VerticalAlignment = VerticalAlignment.Stretch;
            Focusable         = false;
            Width             = 10;

            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);

            _diagnosticService.DiagnosticsChanging += OnDiagnosticsChanging;
            _diagnosticService.DiagnosticsChanged  += OnDiagnosticsChanged;
            _editorFormatMap.FormatMappingChanged  += OnFormatMappingChanged;
            _scrollBar.TrackSpanChanged            += OnTrackSpanChanged;
            _textView.LayoutChanged                += OnTextViewLayoutChanged;
            _textView.Closed                       += OnTextViewClosed;
        }

        void OnTextViewClosed(object sender, EventArgs e) {
            Dispose();
        }

        void OnFormatMappingChanged(object sender, FormatItemsEventArgs e) {

        }

        void OnDiagnosticsChanging(object sender, EventArgs e) {
        }

        void OnDiagnosticsChanged(object sender, EventArgs e) {
            InvalidateGeometry();
            InvalidateVisual();
        }

        void OnTextViewLayoutChanged(object sender, TextViewLayoutChangedEventArgs e) {
            InvalidateGeometry();
            InvalidateVisual();
        }

        void OnTrackSpanChanged(object sender, EventArgs e) {
            InvalidateGeometry();
            InvalidateVisual();
        }

        void InvalidateGeometry() {
            _markerGeometryGroups = null;
        }

        void EnsureGeometry() {

            if (_markerGeometryGroups != null) {
                return;
            }

            _markerGeometryGroups = new Dictionary<DiagnosticSeverity, GeometryGroup>();

            var severities = new[] {
                DiagnosticSeverity.Error,
                DiagnosticSeverity.Warning,
                DiagnosticSeverity.Suggestion
            };

            foreach (var severity in severities) {

                var group = new GeometryGroup();

                foreach (var mappingTagSpan in _diagnosticService.GetDiagnosticsWithSeverity(severity)) {

                    var tagSpan = _textView.MapToSingleSnapshotSpan(mappingTagSpan);
                    if (tagSpan == null) {
                        continue;
                    }

                    group.Children.Add(GetMarkerGeometry(tagSpan.Span.Start));
                }

                _markerGeometryGroups.Add(severity, group);
            }
        }

        Geometry GetMarkerGeometry(SnapshotPoint snapshotPoint) {

            double markerHeight = 2.0;

            var y = _scrollBar.GetYCoordinateOfBufferPosition(snapshotPoint);

            var rectangleGeometry = new RectangleGeometry(new Rect(0, y - markerHeight / 2, Width, markerHeight));

            return rectangleGeometry;
        }

        protected override Size ArrangeOverride(Size finalSize) {
            InvalidateGeometry();
            return base.ArrangeOverride(finalSize);
        }

        protected override void OnRender(DrawingContext dc) {
            base.OnRender(dc);
            EnsureGeometry();

            foreach (var geoGroup in _markerGeometryGroups) {

                var brush = GetMarkerBrush(geoGroup.Key);
                dc.DrawGeometry(brush, null, geoGroup.Value);
            }
        }

        Brush GetMarkerBrush(DiagnosticSeverity severity) {
            // TODO Farben
            switch (severity) {
                case DiagnosticSeverity.Suggestion:
                    return GetForeGroundColor(DiagnosticErrorTypeNames.Suggestion, Brushes.Blue);
                case DiagnosticSeverity.Warning:
                    return Brushes.Orange; //GetForeGroundColor(DiagnosticErrorTypeNames.Warning, Brushes.Orange);
                case DiagnosticSeverity.Error:
                    return GetForeGroundColor(DiagnosticErrorTypeNames.Error, Brushes.Red);
                default:
                    throw new ArgumentOutOfRangeException(nameof(severity), severity, null);
            }
        }

        Brush GetForeGroundColor(string type, Brush fallbackBrush) {
            ResourceDictionary resourceDictionary = _editorFormatMap.GetProperties(type);

            if (resourceDictionary.Contains(EditorFormatDefinition.ForegroundBrushId)) {
                var color = (Brush) resourceDictionary[EditorFormatDefinition.ForegroundBrushId];
                return color;
            } else {
                return fallbackBrush;
            }
        }

        public FrameworkElement VisualElement {
            get {
                ThrowIfDisposed();
                return this;
            }
        }

        public double MarginSize {
            get {
                ThrowIfDisposed();

                return ActualWidth;
            }
        }

        bool ITextViewMargin.Enabled {
            get { return _textView.Options.GetOptionValue(DefaultTextViewHostOptions.VerticalScrollBarId); }
        }

        public ITextViewMargin GetTextViewMargin(string marginName) {
            return string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        public void Dispose() {
            if (!_isDisposed) {
                _isDisposed = true;

                _diagnosticService.DiagnosticsChanging -= OnDiagnosticsChanging;
                _diagnosticService.DiagnosticsChanged  -= OnDiagnosticsChanged;
                _scrollBar.TrackSpanChanged            -= OnTrackSpanChanged;
                _textView.LayoutChanged                -= OnTextViewLayoutChanged;
                _editorFormatMap.FormatMappingChanged  -= OnFormatMappingChanged;
                _textView.Closed                       -= OnTextViewClosed;
            }
        }

        void ThrowIfDisposed() {
            if (_isDisposed) {
                throw new ObjectDisposedException(MarginName);
            }
        }

    }

}