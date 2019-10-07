using System;
using System.Runtime.InteropServices;

using JetBrains.Annotations;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

using Pharmatechnik.Language.Gd.Extension.Classification;

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    [Guid(WindowGuidString)]
    public class GdOutlineToolWindowPane: ToolWindowPane {

        public const string WindowGuidString = "7e927358-0b4d-4953-b2bb-48ef216eb8cb";

        private readonly GdOutlineControl    _outlineControl;
        private readonly GdOutlineController _outlineController;

        public GdOutlineToolWindowPane(TextBlockBuilderService textBlockBuilderService): base(null) {

            BitmapImageMoniker = KnownMonikers.DocumentOutline;

            _outlineControl                         =  new GdOutlineControl(textBlockBuilderService);
            _outlineControl.IsVisibleChanged        += OnOutlineControlIsVisibleChanged;
            _outlineControl.RequestNavigateToSource += OnRequestNavigateToSource;

            _outlineController                          =  new GdOutlineController();
            _outlineController.RequestNavigateToOutline += OnRequestNavigateToOutline;
            _outlineController.OutlineDataChanged       += OnOutlineDataChanged;

            UpdateCaption();
        }

        protected override void Dispose(bool disposing) {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.Dispose(disposing);

            _outlineControl.IsVisibleChanged            -= OnOutlineControlIsVisibleChanged;
            _outlineControl.RequestNavigateToSource     -= OnRequestNavigateToSource;
            _outlineController.RequestNavigateToOutline -= OnRequestNavigateToOutline;
            _outlineController.OutlineDataChanged       -= OnOutlineDataChanged;

            _outlineController.Dispose();
        }

        public override object Content {
            get => _outlineControl;
            set { }
        }

        void OnRequestNavigateToSource(object sender, RequestNavigateToEventArgs e) {
            _outlineController.NavigateToSource(e.OutlineElement);
        }

        void OnRequestNavigateToOutline(object sender, NavigateToOutlineEventArgs e) {
            _outlineControl.NavigateToOutline(e.OutlineElement);
        }

        void OnOutlineControlIsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
            ThreadHelper.ThrowIfNotOnUIThread();

            if (_outlineControl.IsVisible) {
                _outlineController.Run();
            } else {
                _outlineController.Stop();
            }
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

        public const string DefaultCaption = "Gui Outline";

        private void UpdateCaption([CanBeNull] OutlineData outlineData = null) {

           

            var file = outlineData?.SyntaxTree.SourceText.FileInfo?.Name;
            if (!String.IsNullOrEmpty(file)) {
                Caption = $"{DefaultCaption} - {file}";
            } else {
                Caption = DefaultCaption;
            }
        }

    }

}