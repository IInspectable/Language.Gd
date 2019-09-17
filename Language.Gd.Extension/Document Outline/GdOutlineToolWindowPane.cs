using System;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    [Guid("7e927358-0b4d-4953-b2bb-48ef216eb8cb")]
    public class GdOutlineToolWindowPane: ToolWindowPane {

        private readonly GdOutlineControl    _outlineControl;
        private readonly GdOutlineController _outlineController;

        public GdOutlineToolWindowPane(): base(null) {

            Caption            = "Gui Outline";
            BitmapImageMoniker = KnownMonikers.DocumentOutline;
            _outlineControl    = new GdOutlineControl();
            _outlineController = new GdOutlineController();

            _outlineController.ModelChanged += OnModelChanged;
        }

        protected override void Dispose(bool disposing) {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.Dispose(disposing);

            _outlineController.ModelChanged -= OnModelChanged;
            _outlineController.Dispose();
        }

        public override void OnToolWindowCreated() {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.OnToolWindowCreated();
            _outlineController.Run();
        }

        private void OnModelChanged(object sender, GdOutlineEventArgs e) {
            _outlineControl.TxtTest.Text = $"{FormatArgs(e)}";
        }

        string FormatArgs(GdOutlineEventArgs args) {

            if (args.SyntaxTreeAndSnapshot == null) {
                return "There are no items to show for the selected document.";
            }

            var fileName = args.SyntaxTreeAndSnapshot.SyntaxTree.SourceText.FileInfo?.Name ?? "<unknown file>";

            return $"{args.ActivePosition} {DateTime.Now.ToLongTimeString()} ''{fileName}''";
        }

        public override object Content {
            get => _outlineControl;
            set { }
        }

    }

}