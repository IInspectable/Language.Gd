#region Using Directives

using System;
using System.Windows;
using System.Windows.Media;

using Microsoft.VisualStudio.Text.Editor;

#endregion

namespace Pharmatechnik.Language.Gd.Extension.Margin {

    sealed class GdMargin: IWpfTextViewMargin {

        public const string MarginName = GdLanguageContentDefinitions.ContentType + nameof(GdMargin);

        private  bool         _isDisposed;
        readonly IWpfTextView _textView;

        readonly GdMarginControl _marginControl;

        public GdMargin(IWpfTextView textView) {
            _textView = textView;

            _marginControl = new GdMarginControl(textView);

            _textView.Closed += OnTextViewClosed;

            RenderOptions.SetEdgeMode(_marginControl, EdgeMode.Aliased);

        }

        public void Dispose() {
            if (_isDisposed) {
                return;
            }

            _isDisposed = true;

            _textView.Closed -= OnTextViewClosed;
        }

        private void OnTextViewClosed(object sender, EventArgs e) {
            Dispose();
        }

        public ITextViewMargin GetTextViewMargin(string marginName) {
            return string.Equals(marginName, MarginName, StringComparison.OrdinalIgnoreCase) ? this : null;
        }

        public double MarginSize => _marginControl.ActualWidth;

        public bool Enabled => true;

        public FrameworkElement VisualElement => _marginControl;

    }

}