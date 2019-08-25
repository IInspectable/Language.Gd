#region Using Directives

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using Microsoft.VisualStudio.Text.Projection;

using Pharmatechnik.Language.Gd.Extension.ParserService;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Common {

    [Export(typeof(CodeContentControlProvider))]
    sealed class CodeContentControlProvider {

        #region Fields/Ctor

        readonly ITextEditorFactoryService       _textEditorFactory;
        readonly IProjectionBufferFactoryService _projectionFactory;
        readonly TextViewConnectionListener      _textViewConnectionListener;
        readonly ITextBufferFactoryService       _textBufferFactoryService;
        readonly ITextEditorFactoryService       _textEditorFactoryService;

        [ImportingConstructor]
        public CodeContentControlProvider(ITextEditorFactoryService textEditorFactory,
                                          IProjectionBufferFactoryService projectionFactory,
                                          TextViewConnectionListener textViewConnectionListener,
                                          ITextBufferFactoryService textBufferFactoryService,
                                          ITextEditorFactoryService textEditorFactoryService) {

            _textEditorFactory          = textEditorFactory;
            _projectionFactory          = projectionFactory;
            _textViewConnectionListener = textViewConnectionListener;
            _textBufferFactoryService   = textBufferFactoryService;
            _textEditorFactoryService   = textEditorFactoryService;
        }

        #endregion

        public const string CodeContentTextViewRole = nameof(CodeContentTextViewRole);

        public ContentControl CreateContentControlForOutlining(SnapshotSpan span) {
            return new CodePreviewControl(() => CreateTextView(span));
        }

        public ContentControl CreateContentControlForQuickInfo(ITextBuffer textBuffer, string navCode, ParseMethod parseMethod) {
            return new CodePreviewControl(() => CreateTextView(textBuffer, navCode, parseMethod));
        }

        IWpfTextView CreateTextView(ITextBuffer textBuffer, string navCode, ParseMethod parseMethod) {

            var buffer = _textBufferFactoryService.CreateTextBuffer(navCode, textBuffer.ContentType);

            ParserService.ParserService.SetParseMethod(buffer, parseMethod);

            var roles = _textEditorFactoryService.CreateTextViewRoleSet(CodeContentTextViewRole);
            var view  = _textEditorFactory.CreateTextView(buffer, roles);

            view.Background =  Brushes.Transparent;
            view.ZoomLevel  *= 0.75;

            view.PrepareSizeToFit();

            return view;
        }

        IWpfTextView CreateTextView(SnapshotSpan span) {

            var buffer = CreatePreviewBuffer(span);
            var roles  = _textEditorFactoryService.CreateTextViewRoleSet(CodeContentTextViewRole);
            var view   = _textEditorFactory.CreateTextView(buffer, roles);

            view.Background =  Brushes.Transparent;
            view.ZoomLevel  *= 0.75;

            view.PrepareSizeToFit();

            return view;
        }

        ITextBuffer CreatePreviewBuffer(SnapshotSpan span) {

            var exposedSpans = GetPreviewSpans(span, 23, out bool needsEllipses);

            ITextBuffer buffer = _projectionFactory.CreateElisionBuffer(null, exposedSpans, ElisionBufferOptions.None);
            if (needsEllipses) {
                buffer = CreateProjectionBufferWithEllipses(buffer);
            }

            return buffer;
        }

        ITextBuffer CreateProjectionBufferWithEllipses(ITextBuffer elisionBuffer) {
            // The elision buffer is too long.  We've already trimmed it, but now we want to add
            // a "..." to it.  We do that by creating a projection of both the elision buffer and
            // a new text buffer wrapping the ellipsis.
            var elisionSpan = elisionBuffer.CurrentSnapshot.GetFullSpan();

            var sourceSpans = new List<object>() {
                elisionSpan.Snapshot.CreateTrackingSpan(elisionSpan, SpanTrackingMode.EdgeExclusive),
                "..."
            };

            var projectionBuffer = _projectionFactory.CreateProjectionBuffer(
                projectionEditResolver: null,
                sourceSpans: sourceSpans,
                options: ProjectionBufferOptions.None);

            return projectionBuffer;
        }

        #region Dokumenation

        /// <summary>
        /// Liefert die Bereiche für eine Vorschau im Tooltip.
        /// </summary>
        /// <example>
        /// 
        /// o : Leehrzeichen
        /// ->: Tabulator (weite 4)
        /// T : beliebiger Text
        /// » : Begin der Region
        /// « : Ende der Region
        /// 
        /// ToTTTT->»TTTTTTT
        /// oo->--->TTTTTTTT
        /// ooooooooooTTTTTT
        /// ooooooooTTTTTTTT«
        /// TTTTTTTTTTTTTTTT
        /// 
        /// ToTTTT->|TTTTTTTT
        /// oo->--->|TTTTTTTT
        /// oooooooo|ooTTTTTT
        /// oooooooo|TTTTTTTT
        /// --------^ (signifikante Spalte)
        /// 
        /// Der Text, wie er in der Vorschau angezeigt wird:
        /// TTTTTTTT
        /// TTTTTTTT
        /// ooTTTTTT
        /// TTTTTTTT
        /// </example>

        #endregion

        NormalizedSnapshotSpanCollection GetPreviewSpans(SnapshotSpan span, int maxLines, out bool shortened) {

            var parentView = _textViewConnectionListener.GetTextViewForBuffer(span.Snapshot.TextBuffer);

            var startLineIndex = span.Start.GetContainingLine().LineNumber;
            var endLineIndex   = span.End.GetContainingLine().LineNumber;
            var lineCount      = endLineIndex - startLineIndex + 1;

            shortened = false;
            maxLines  = Math.Min(maxLines, lineCount);

            if (lineCount > maxLines) {
                shortened    = true;
                lineCount    = maxLines;
                endLineIndex = startLineIndex + lineCount - 1;

                var newEnd = span.Snapshot.GetLineFromLineNumber(endLineIndex).EndIncludingLineBreak;
                span = new SnapshotSpan(span.Start, newEnd);
            }

            var tabSize           = parentView.Options.GetTabSize();
            var significantColumn = Int32.MaxValue;
            var lines             = new List<ITextSnapshotLine>();
            for (int lineIndex = startLineIndex; lineIndex <= endLineIndex; lineIndex++) {

                var line        = span.Snapshot.GetLineFromLineNumber(lineIndex);
                var isFirstLine = lineIndex == startLineIndex;
                if (isFirstLine) {
                    significantColumn = line.GetColumnForOffset(tabSize, span.Start - line.Start);
                } else {
                    significantColumn = Math.Min(significantColumn, line.GetSignificantColumn(tabSize));
                }

                lines.Add(line);
            }

            var result = new List<SnapshotSpan>();
            for (int lineIndex = 0; lineIndex < lines.Count; lineIndex++) {

                var line        = lines[lineIndex];
                var isFirstLine = lineIndex == 0;
                var isLastLine  = lineIndex == lines.Count - 1;
                int offset      = line.GetOffsetForColumn(significantColumn, tabSize);

                if (isFirstLine) {
                    offset = span.Start - line.Start;
                }

                var end = isLastLine ? span.End : line.EndIncludingLineBreak;
                result.Add(new SnapshotSpan(line.Start + offset, end));
            }

            return new NormalizedSnapshotSpanCollection(result);
        }

        #region CodePreviewControl

        sealed class CodePreviewControl: ContentControl {

            readonly Func<IWpfTextView> _createView;

            ITextView TextView {
                get {
                    var wpfTextView = (IWpfTextView) Content;
                    if (wpfTextView == null) {
                        wpfTextView = _createView();
                        Content     = wpfTextView.VisualElement;
                    }

                    return wpfTextView;
                }
            }

            public CodePreviewControl(Func<IWpfTextView> createView) {
                _createView         =  createView;
                IsVisibleChanged    += OnIsVisibleChanged;
                Background          =  Brushes.Transparent;
                HorizontalAlignment =  HorizontalAlignment.Left;
            }

            void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
                if ((bool) e.NewValue) {
                    if (Content == null)
                        Content = _createView().VisualElement;
                } else {
                    TextView.Close();
                    Content = null;
                }
            }

        }

        #endregion

    }

}