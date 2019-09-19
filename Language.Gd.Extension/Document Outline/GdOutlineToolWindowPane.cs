using System;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    [Guid("7e927358-0b4d-4953-b2bb-48ef216eb8cb")]
    public class GdOutlineToolWindowPane: ToolWindowPane {

        private readonly GdOutlineControl    _outlineControl;
        private readonly GdOutlineController _outlineController;

        public GdOutlineToolWindowPane(): base(null) {

            BitmapImageMoniker = KnownMonikers.DocumentOutline;
            _outlineControl    = new GdOutlineControl();
            _outlineController = new GdOutlineController();

            _outlineControl.RequestNavigateToSource     += OnRequestNavigateToSource;
            _outlineController.RequestNavigateToOutline += OnRequestNavigateToOutline;
            _outlineController.OutlineDataChanged       += OnOutlineDataChanged;

            UpdateCaption();
        }

        protected override void Dispose(bool disposing) {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.Dispose(disposing);

            _outlineControl.RequestNavigateToSource     -= OnRequestNavigateToSource;
            _outlineController.RequestNavigateToOutline -= OnRequestNavigateToOutline;
            _outlineController.OutlineDataChanged       -= OnOutlineDataChanged;

            _outlineController.Dispose();
        }

        public override object Content {
            get => _outlineControl;
            set { }
        }

        private void OnRequestNavigateToOutline(object sender, NavigateToOutlineEventArgs e) {
            _outlineControl.NavigateToOutline(e.OutlineElement);
        }

        private void OnRequestNavigateToSource(object sender, RequestNavigateToEventArgs e) {
            _outlineController.NavigateToSource(e.OutlineElement);
        }

        public override void OnToolWindowCreated() {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.OnToolWindowCreated();
            _outlineController.Run();
        }

        private void OnOutlineDataChanged(object sender, OutlineDataEventArgs e) {

            _outlineControl.ShowOutline(e.OutlineData);

            UpdateCaption(e.OutlineData);
        }

        private void UpdateCaption([CanBeNull] OutlineData outlineData = null) {

            const string defaultCaption = "Gui Outline";

            var file = outlineData?.SyntaxTree.SourceText.FileInfo?.Name;
            if (!String.IsNullOrEmpty(file)) {
                Caption = $"{defaultCaption} - {file}";
            } else {
                Caption = defaultCaption;
            }
        }

    }

}