﻿using System;
using System.Runtime.InteropServices;

using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Shell;

namespace Pharmatechnik.Language.Gd.Extension.Document_Outline {

    [Guid("7e927358-0b4d-4953-b2bb-48ef216eb8cb")]
    public class GdOutlineToolWindowPane: ToolWindowPane {

        private readonly GdOutlineControl    _outlineControl;
        private readonly GdOutlineController _outlineController;

        public GdOutlineToolWindowPane(): base(null) {

            Caption            = DefaultCaption;
            BitmapImageMoniker = KnownMonikers.DocumentOutline;
            _outlineControl    = new GdOutlineControl();
            _outlineController = new GdOutlineController();

            _outlineController.OutlineDataChanged += OnOutlineDataChanged;
        }

        protected override void Dispose(bool disposing) {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.Dispose(disposing);

            _outlineController.OutlineDataChanged -= OnOutlineDataChanged;
            _outlineController.Dispose();
        }

        public override void OnToolWindowCreated() {
            ThreadHelper.ThrowIfNotOnUIThread();

            base.OnToolWindowCreated();
            _outlineController.Run();
        }

        private void OnOutlineDataChanged(object sender, GdOutlineEventArgs e) {
            _outlineControl.TxtTest.Text = $"{FormatArgs(e)}";
           
            var file = e.OutlineData?.SyntaxTree.SourceText.FileInfo?.Name;
            if (!String.IsNullOrEmpty(file)) {
                Caption = $"{DefaultCaption} - {file}";
            } else {
                Caption = DefaultCaption;
            }
        }

        private static string DefaultCaption => "Gui Outline";

        string FormatArgs(GdOutlineEventArgs args) {

            if (args.OutlineData == null) {
                return "There are no items to show for the selected document.";
            }

            var elem = args.OutlineData.OutlineElement.FindElement(args.OutlineData.ActivePosition??0);

            return $"{args.OutlineData.ActivePosition} '{elem?.DisplayName}' {DateTime.Now.ToLongTimeString()}";
        }

        public override object Content {
            get => _outlineControl;
            set { }
        }

    }

}